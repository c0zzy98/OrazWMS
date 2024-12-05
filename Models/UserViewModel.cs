namespace OrazWMS.Models
{
    public class UserViewModel
    {
        public int Id { get; set; } // Numer porządkowy
        public string Email { get; set; } // Adres email użytkownika
        public string Role { get; set; } // Rola użytkownika
        public DateTime CreatedAt { get; set; } // Data utworzenia użytkownika
    }
}
