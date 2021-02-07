using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Controllers;

namespace SportsStore.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void CanUseRepository()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" },
                }).AsQueryable<Product>()
            );

            HomeController controller = new(mock.Object);

            ProductListViewModel result =
                (controller.Index(null) as ViewResult).ViewData.Model as ProductListViewModel;

            Product[] prodArr = result.Products.ToArray();
            Assert.True(prodArr.Length == 2);
            Assert.Equal("P1", prodArr[0].Name);
            Assert.Equal("P2", prodArr[1].Name);
        }

        [Fact]
        public void CanPaginate()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" },
                    new Product { ProductID = 3, Name = "P3" },
                    new Product { ProductID = 4, Name = "P4" },
                    new Product { ProductID = 5, Name = "P5" },
                }).AsQueryable<Product>()
            );

            HomeController controller = new(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result =
                (controller.Index(null, 2) as ViewResult).ViewData.Model as ProductListViewModel;

            Product[] prodArr = result.Products.ToArray();
            Assert.True(prodArr.Length == 2);
            Assert.Equal("P4", prodArr[0].Name);
            Assert.Equal("P5", prodArr[1].Name);
        }

        [Fact]
        public void CanSendPaginationViewModel()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1" },
                    new Product { ProductID = 2, Name = "P2" },
                    new Product { ProductID = 3, Name = "P3" },
                    new Product { ProductID = 4, Name = "P4" },
                    new Product { ProductID = 5, Name = "P5" },
                }).AsQueryable<Product>()
            );

            HomeController controller = new(mock.Object) { PageSize = 3 };
            
            ProductListViewModel result =
                (controller.Index(null, 2) as ViewResult).ViewData.Model as ProductListViewModel;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void CanFilterProducts()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                    new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                    new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
                    new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
                    new Product { ProductID = 5, Name = "P5", Category = "Cat3" },
                }).AsQueryable<Product>()
            );

            HomeController controller = new(mock.Object);
            controller.PageSize = 3;

            Product[] result =
                ((controller.Index("Cat2", 1) as ViewResult).ViewData.Model as ProductListViewModel).Products.ToArray();
            
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [Fact]
        public void GenerateCategorySpecificProductCount()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
                    new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
                    new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
                    new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
                    new Product { ProductID = 5, Name = "P5", Category = "Cat3" },
                }).AsQueryable<Product>()
            );

            HomeController target = new(mock.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductListViewModel> getModel = result =>
                result?.ViewData?.Model as ProductListViewModel;

            int? res1 = getModel(target.Index("Cat1") as ViewResult)?.PagingInfo.TotalItems;
            int? res2 = getModel(target.Index("Cat2") as ViewResult)?.PagingInfo.TotalItems;
            int? res3 = getModel(target.Index("Cat3") as ViewResult)?.PagingInfo.TotalItems;
            int? resAll = getModel(target.Index(null) as ViewResult)?.PagingInfo.TotalItems;

            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}
