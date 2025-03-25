using ETicaretDal.Abstract;
using ETicaretData.Entities;
using ETicaretData.Helpers;
using ETicaretData.Validators;
using ETicaretData.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Globalization;

namespace EticartUI.Controllers
{
    public class CardController : Controller
    {
        private readonly IOrderDal _orderDal;
        private readonly IProductDal _productDal;
        private readonly IValidator<ShippingDetails> _validator;

        public CardController(IOrderDal orderDal, IProductDal productDal, IValidator<ShippingDetails> validator)
        {
            _orderDal = orderDal;
            _productDal = productDal;
            _validator = validator;
        }

        public IActionResult Index()
        {
            var card = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if (card==null)
            {
                return View();
            }
            ViewBag.Total = string.Format(CultureInfo.CurrentCulture, "{0:C}", card.Sum(x => x.Product.Price * x.Quantity));
            SessionHelper.Count = card.Count;
            return View(card);
        }
        public IActionResult Buy(int id)
        {
            if (SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card") ==null)
            {
                var cart = new List<CardItem>();
                cart.Add(new CardItem { Product = _productDal.Get(id), Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
                SessionHelper.Count = 1;
            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
                //
                int index = isExits(cart, id);
                if (index<0)
                {
                    cart.Add(new CardItem { Product = _productDal.Get(id), Quantity = 1 });
                }
                else
                {
                    cart[index].Quantity++;
                }
                //tekrar güncelle
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
                SessionHelper.Count = cart.Count;
            }
            return Content(@"<script>
                document.querySelector('.cart-count').textContent = '" + SessionHelper.Count + @"';
                var notification = document.createElement('div');
                notification.style.position = 'fixed';
                notification.style.top = '20px';
                notification.style.right = '20px';
                notification.style.backgroundColor = '#28a745';
                notification.style.color = 'white';
                notification.style.padding = '10px 20px';
                notification.style.borderRadius = '5px';
                notification.style.zIndex = '1000';
                notification.textContent = 'Ürün sepete eklendi';
                document.body.appendChild(notification);
                setTimeout(function() {
                    notification.style.opacity = '0';
                    notification.style.transition = 'opacity 0.5s';
                    setTimeout(function() {
                        notification.remove();
                    }, 500);
                }, 2000);
            </script>", "text/html");
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if (Cart == null||Cart.Count<1)
            {
                ModelState.AddModelError("Urun Yok", "Sepetinizden Urun yok..");
                return RedirectToAction("Index", "Home");
            }

            return View(new ShippingDetails());

        }
        [HttpPost]
        public IActionResult CheckOut(ShippingDetails detail)
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if (Cart == null)
            {
                ModelState.AddModelError("", "Sepetinizde ürün bulunmamaktadır.");
                return View(detail);
            }

            ValidationResult results = _validator.Validate(detail);

            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(detail);
            }

            SaveOrder(Cart, detail);
            Cart.Clear();
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
            TempData["Message"] = "Siparişiniz başarıyla alındı.";
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            int index = isExits(Cart, id);
            Cart.RemoveAt(index);
            if (Cart.Count==0)
            {
                Cart =null;
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
            
            return RedirectToAction("Index");
        }
        private void SaveOrder(List<CardItem>? cart, ShippingDetails detail)
        {
            var quint = "A"+(new Random().Next(1111, 99999).ToString());
            var order = new Order();

            order.OrderNumber = quint;
            
            order.Total = cart.Sum(i => i.Product.Price * i.Quantity);
            order.OrderDate = DateTime.Now;
            order.orderState = EnumOrderState.Waitig;
            order.UserName = detail.UserName;
            order.Adress = detail.Adress;
            order.AdressTitle = detail.AdressTitle;
            order.City = detail.City;

            order.OrderLines = new List<OrderLine>();

            foreach (var item in cart)
            {
                var orderline = new OrderLine();

                orderline.Quantity = item.Quantity;
                orderline.Price = item.Quantity * item.Product.Price;
                orderline.ProductId = item.Product.Id;
                order.OrderLines.Add(orderline);
            }
            _orderDal.Add(order);
        }

        private int isExits(List<CardItem> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                    
                }
               
            }
            return -1;
        }
        
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int change)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if (cart != null)
            {
                int index = isExits(cart, id);
                if (index >= 0)
                {
                    // Yeni miktar hesapla
                    int newQuantity = cart[index].Quantity + change;
                    
                    // Miktar 0'dan küçük olamaz
                    if (newQuantity <= 0)
                    {
                        cart.RemoveAt(index);
                    }
                    else
                    {
                        cart[index].Quantity = newQuantity;
                    }

                    // Session'ı güncelle
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
                    SessionHelper.Count = cart.Count;

                    // Yeni toplam fiyatı hesapla
                    decimal total = cart.Sum(x => x.Product.Price * x.Quantity);
                    decimal itemTotal = cart[index]?.Product.Price * newQuantity ?? 0;

                    // JSON response döndür
                    return Json(new { 
                        success = true, 
                        quantity = newQuantity > 0 ? newQuantity : 0,
                        total = string.Format(CultureInfo.CurrentCulture, "{0:C}", total),
                        itemTotal = string.Format(CultureInfo.CurrentCulture, "{0:C}", itemTotal),
                        cartCount = cart.Count,
                        removed = newQuantity <= 0
                    });
                }
            }
            return Json(new { success = false });
        }
    }
}
