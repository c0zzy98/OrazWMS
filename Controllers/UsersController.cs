using Microsoft.AspNetCore.Mvc;
using OrazWMS.Models;

namespace OrazWMS.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            // Przykładowe dane - do późniejszego zastąpienia danymi z bazy
            var users = new List<UserViewModel>
            {
                new UserViewModel
                {
                    Id = 1,
                    Email = "admin@example.com",
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                },
                new UserViewModel
                {
                    Id = 2,
                    Email = "user@example.com",
                    Role = "User",
                    CreatedAt = DateTime.Now.AddDays(-7)
                }
            };

            // Przekazanie listy użytkowników do widoku
            return View(users);
        }
    }
}
