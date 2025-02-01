using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Data;
using OrazWMS.Models; // Poprawne użycie ApplicationUser

namespace OrazWMS.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Select(u => new UserViewModel
                {
                    Name = u.UserName,
                    Email = u.Email,
                    Phone = u.PhoneNumber ?? "Brak",
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault() ?? "Brak roli",
                    CreatedAt = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();

            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = roles;

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser // Poprawione użycie ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role) && await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, message = "Nie udało się dodać użytkownika." });
        }
    }
}
