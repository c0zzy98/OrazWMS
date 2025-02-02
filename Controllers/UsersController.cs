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
                    Id = u.Id,
                    UserName = u.UserName,
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
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Nieprawidłowe dane wejściowe." });
            }

            try
            {
                // **SPRAWDZENIE, CZY EMAIL LUB USERNAME JUŻ ISTNIEJE**
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);

                if (existingUserByEmail != null)
                {
                    return Json(new { success = false, message = "Użytkownik z tym adresem email już istnieje." });
                }

                if (existingUserByUsername != null)
                {
                    return Json(new { success = false, message = "Nazwa użytkownika jest już zajęta." });
                }

                // **TWORZENIE NOWEGO UŻYTKOWNIKA**
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Json(new { success = false, message = "Błąd tworzenia użytkownika", errors });
                }

                // **SPRAWDZENIE I PRZYPISANIE ROLI**
                if (!string.IsNullOrEmpty(model.Role))
                {
                    var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                    if (!roleExists)
                    {
                        return Json(new { success = false, message = $"Rola '{model.Role}' nie istnieje." });
                    }

                    var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                        return Json(new { success = false, message = "Błąd przypisywania roli", errors = roleErrors });
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Wystąpił błąd: {ex.Message}" });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { success = false, message = "Nieprawidłowy identyfikator użytkownika." });
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Użytkownik nie istnieje." });
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized(new { success = false, message = "Brak uprawnień do wykonania tej operacji." });
                }

                bool isTargetAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                bool isCurrentAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

                if (!isCurrentAdmin && isTargetAdmin)
                {
                    return StatusCode(403, new { success = false, message = "Nie masz uprawnień do usunięcia administratora." });
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Nie udało się usunąć użytkownika.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { success = true, message = "Użytkownik został pomyślnie usunięty." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Wystąpił nieoczekiwany błąd podczas usuwania użytkownika.",
                    error = ex.Message
                });
            }
        }
    }
}
