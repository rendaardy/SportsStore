using System.Linq;
using Xunit;
using SportsStore.Models;

namespace SportsStore.Tests
{
    public class CartTests
    {
        [Fact]
        public void CanAddNewLines()
        {
            Product p1 = new() { ProductID = 1, Name = "P1" };
            Product p2 = new() { ProductID = 2, Name = "P2" };

            Cart target = new();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            Assert.Equal(2, results.Length);
            Assert.Equal(p1, results[0].Product);
            Assert.Equal(p2, results[1].Product);
        }

        [Fact]
        public void CanAddQuantityForExistingLines()
        {
            Product p1 = new() { ProductID = 1, Name = "P1" };
            Product p2 = new() { ProductID = 2, Name = "P2" };

            Cart target = new();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines
                .OrderBy(l => l.Product.ProductID).ToArray();

            Assert.Equal(2, results.Length);
            Assert.Equal(11, results[0].Quantity);
            Assert.Equal(1, results[1].Quantity);
        }

        [Fact]
        public void CanRemoveLine()
        {
            Product p1 = new() { ProductID = 1, Name = "P1" };
            Product p2 = new() { ProductID = 2, Name = "P2" };
            Product p3 = new() { ProductID = 3, Name = "P3" };

            Cart target = new();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            target.RemoveLine(p2);

            Assert.Empty(target.Lines.Where(l => l.Product == p2));
            Assert.Equal(2, target.Lines.Count());
        }

        [Fact]
        public void CalculateCartTotal()
        {
            Product p1 = new() { ProductID = 1, Name = "P1", Price = 100.0 };
            Product p2 = new() { ProductID = 2, Name = "P2", Price = 50.0 };

            Cart target = new();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            decimal result = target.ComputeTotalValue();

            Assert.Equal(450m, result);
        }

        [Fact]
        public void CanClearContents()
        {
            Product p1 = new() { ProductID = 1, Name = "P1", Price = 100.0 };
            Product p2 = new() { ProductID = 2, Name = "P2", Price = 50.0 };

            Cart target = new();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            Assert.Empty(target.Lines);
        }
    }
}
