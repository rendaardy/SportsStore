using System.Text;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using SportsStore.Models;
using SportsStore.Pages;

namespace SportsStore.Tests
{
    public class CartPageTests
    {
        [Fact]
        public void CanLoadCart()
        {
            Product p1 = new() { ProductID = 1, Name = "P1" };
            Product p2 = new() { ProductID = 2, Name = "P2" };
            Mock<IStoreRepository> mockRepo = new();
            mockRepo.Setup(m => m.Products).Returns(
                (new Product[] { p1, p2 }).AsQueryable<Product>()
            );

            Cart testCart = new();
            testCart.AddItem(p1, 1);
            testCart.AddItem(p2, 1);

            CartModel cartModel = new(mockRepo.Object, testCart);
            cartModel.OnGet("myUrl");

            Assert.Equal(2, cartModel.Cart.Lines.Count());
            Assert.Equal("myUrl", cartModel.ReturnUrl);
        }

        [Fact]
        public void CanUpdateCart()
        {
            Mock<IStoreRepository> mockRepo = new();
            mockRepo.Setup(m => m.Products).Returns(
                (new Product[] {
                    new Product { ProductID = 1, Name = "P1" }
                }).AsQueryable<Product>()
            );

            Cart testCart = new();

            CartModel cartModel = new(mockRepo.Object, testCart);
            cartModel.OnPost(1, "myUrl");

            Assert.Single(testCart.Lines);
            Assert.Equal("P1", testCart.Lines.First().Product.Name);
            Assert.Equal(1, testCart.Lines.First().Quantity);
        }
    }
}
