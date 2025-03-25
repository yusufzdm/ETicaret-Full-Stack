using System.Diagnostics;
using ETicaretDal.Abstract;
using ETicaretData.ViewModels;
using EticartUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace EticartUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryDal _categoryDal;
        private readonly IProductDal _productDal;

        public HomeController(ILogger<HomeController> logger, ICategoryDal categoryDal, IProductDal productDal)
        {
            _logger = logger;
            _categoryDal = categoryDal;
            _productDal = productDal;
        }

        public IActionResult Index()
        {
            var products = _productDal.GetAll(p => p.IsHome && p.IsApproved);
            var models = new ListViewModel()
            {
                Categories = _categoryDal.GetAll(),
                Products = products
            };
            return View(models);
        }
        public IActionResult List(int? id)
        {
            ViewBag.ıd = id;
            var product = _productDal.GetAll(x => x.IsApproved);
            if (id!=null)
            {
                product = product.Where(p => p.CategoryId == id).ToList();
            }
            var models = new ListViewModel()
            {
                Categories=_categoryDal.GetAll(),
                Products=product
            };
            return View(models);
        }
        public IActionResult Details(int id)
        {
            var product = _productDal.Get(id);
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
