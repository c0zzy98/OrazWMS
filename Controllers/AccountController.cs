using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrazWMS.Models; // ViewModel dla logowania
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace OrazWMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<OrazWMS.Models.ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<OrazWMS.Models.ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                return RedirectToAction("Index", "Dashboard");
            }

            _logger.LogWarning("Failed login attempt for {Email}.", model.Email);
            ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }
    }
}
