using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Xunit;
using Moq;
using SportsStore.Models;
using SportsStore.Components;

namespace SportsStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void CanSelectCategories()
        {
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                    new Product { ProductID = 2, Name = "P2", Category = "Apples" },
                    new Product { ProductID = 3, Name = "P3", Category = "Plums" },
                    new Product { ProductID = 4, Name = "P4", Category = "Oranges" },
                }).AsQueryable<Product>()
            );

            NavigationMenuViewComponent target = new(mock.Object);

            List<string> results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult)
                .ViewData.Model).ToList();

            Assert.True(results.SequenceEqual(new List<string> { "Apples", 
                "Oranges", "Plums" }));
        }

        [Fact]
        public void IndicatesSelectedCategory()
        {
            string categoryToSelect = "Apples";
            Mock<IStoreRepository> mock = new();
            mock.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1", Category = "Apples" },
                    new Product { ProductID = 4, Name = "P2", Category = "Oranges" },
                }).AsQueryable<Product>()
            );

            NavigationMenuViewComponent target = new(mock.Object);
            target.ViewComponentContext = new ViewComponentContext {
                ViewContext = new ViewContext {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };
            target.RouteData.Values["category"] = categoryToSelect;

            string result = (string)(target.Invoke() as ViewViewComponentResult).ViewData["SelectedCategory"];

            Assert.Equal(categoryToSelect, result);
        }
    }
}
