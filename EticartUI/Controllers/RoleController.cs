﻿using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EticartUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            if (_roleManager.Roles.ToList() == null)
            {
                return NotFound("Roller bullunamadı...");
            }

            return View(_roleManager.Roles.Where(i=>i.Name!="Admin").ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel role )
        {
            var _role = await _roleManager.FindByNameAsync(role.Name);

            if (_role==null)
            {
                var sonuc = await _roleManager.CreateAsync(new AppRole(role.Name));
                if (sonuc.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
         
         

            return View(role);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());
            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();
            var sonuc = await _roleManager.UpdateAsync(role);
            if (sonuc.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var sonuc = await _roleManager.DeleteAsync(role);
            if (sonuc.Succeeded)            
                return RedirectToAction("Index");            
            else
                return NotFound();
        }
    }
}
