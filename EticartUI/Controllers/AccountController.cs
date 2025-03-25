using ETicaretData.Identity;
using ETicaretData.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EticartUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IValidator<LoginViewModel> _loginValidator;
        private readonly IValidator<RegisterViewModel> _registerValidator;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IValidator<LoginViewModel> loginValidator,
            IValidator<RegisterViewModel> registerValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var validationResult = _loginValidator.Validate(login);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(login);
            }

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null)
            {
                var sonuc = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
                if (sonuc.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            }

            return View(login);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            var validationResult = _registerValidator.Validate(register);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(register);
            }

            AppUser user = new AppUser()
            {
                Name = register.FirstName,
                SurName = register.Surname,
                UserName = register.UserName,
                Email = register.Email
            };

            var sonuc = await _userManager.CreateAsync(user, register.Password);
            if (sonuc.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Users");
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var item in sonuc.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(register);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
