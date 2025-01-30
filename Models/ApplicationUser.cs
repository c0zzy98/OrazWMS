using Microsoft.AspNetCore.Identity;
using System;

namespace OrazWMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Domyślnie ustawia datę rejestracji
    }
}
