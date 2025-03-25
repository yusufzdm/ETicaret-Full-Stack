using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ETicartUI.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ETicaretContext _context;

        public OrderController(ETicaretContext context)
        {
            _context = context;
        }


        // GET: Order
        public async Task<IActionResult> Index()
        {
            // Tüm siparişleri getiriyoruz
            var orders = await _context.Orders
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ToListAsync(); // Direkt List

            // Eğer admin değilse, sadece kendi siparişlerini görecek
            if (!User.IsInRole("Admin"))
            {
                var currentUser = User.Identity.Name; // Giriş yapan kullanıcı
                orders = orders.Where(o => o.UserName == currentUser).ToList(); // Bellekte filtreleme
            }

            return View(orders); // Listeyi View'a gönder
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeState(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Enum dropdown için ViewBag ile seçenekler
            ViewBag.OrderStates = Enum.GetValues(typeof(EnumOrderState))
                                      .Cast<EnumOrderState>()
                                      .Select(e => new SelectListItem
                                      {
                                          Value = ((int)e).ToString(),
                                          Text = e.GetType()
                                                  .GetMember(e.ToString())[0]
                                                  .GetCustomAttributes(typeof(DisplayAttribute), false)
                                                  .Cast<DisplayAttribute>()
                                                  .FirstOrDefault()?.Name ?? e.ToString()
                                      });

            return View(order);
        }
        // POST: Order/ChangeState/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeState(int id, EnumOrderState orderState)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return RedirectToAction("Error", "Home");
            }

            order.orderState = orderState;
            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Güncellemeden sonra listeye dön
        }


        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = await _context.Orders.Include(o => o.OrderLines) // Order içindeki OrderLine
                .ThenInclude(ol => ol.Product) // OrderLine içindeki Product
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(order);
        }





        // GET: Order/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Enum listesini gönderiyoruz, ViewData key'i farklı: "OrderStateOptions"
              ViewData["OrderStateSelectList"] = Enum.GetValues(typeof(EnumOrderState))
             .Cast<EnumOrderState>()
             .Select(e => new SelectListItem
             {
                 Value = ((int)e).ToString(),
                 Text = e.GetType().GetMember(e.ToString())[0]
                             .GetCustomAttributes(typeof(DisplayAttribute), false)
                             .Cast<DisplayAttribute>()
                             .FirstOrDefault()?.Name ?? e.ToString()
             }).ToList();


            return View(order);
        }




        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id","OrderNumber","OrderDate","Total","orderState","UserName","AdressTitle","Adress","City")] Order order)
        {
             
            if (id != order.Id)
            {
                return RedirectToAction("Error", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Veritabanı bağlantısı yok.");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
