using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository _repository;

        public int PageSize = 4;

        public HomeController(IStoreRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index(string category, int productPage = 1)
        {
            return View(new ProductListViewModel {
                Products = (from p in _repository.Products
                            where category == null || p.Category == category
                            orderby p.ProductID
                            select p)
                        .Skip((productPage - 1) * PageSize)
                        .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null
                        ? _repository.Products.Count()
                        : _repository.Products.Where(p => p.Category == category).Count()
                },
                CurrentCategory = category,
            });
        }
    }
}
