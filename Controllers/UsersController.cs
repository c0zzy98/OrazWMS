using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Data;
using OrazWMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrazWMS.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .FirstOrDefault() ?? "Brak roli", // Pobieramy rzeczywistą rolę użytkownika
                        CreatedAt = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();

            return View(users);
        }
    }
}
