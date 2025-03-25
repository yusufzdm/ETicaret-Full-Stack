using ETicaretData.Entities;
using ETicaretDal.Abstract;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EticartUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductDal _productDal;
        private readonly ICategoryDal _categoryDal;
        private readonly IValidator<Product> _productValidator;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductDal productDal, ICategoryDal categoryDal, IValidator<Product> productValidator, IWebHostEnvironment webHostEnvironment)
        {
            _productDal = productDal;
            _categoryDal = categoryDal;
            _productValidator = productValidator;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _productDal.GetAllWithCategory();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile? imageFile)
        {
            var validationResult = _productValidator.Validate(product);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name");
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                product.Image = "/images/" + fileName;
            }

            _productDal.Add(product);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var product = _productDal.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? imageFile)
        {
            var validationResult = _productValidator.Validate(product);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                product.Image = "/images/" + fileName;
            }

            _productDal.Update(product);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _productDal.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productDal.GetById(id);
            if (product != null)
            {
                _productDal.Delete(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
