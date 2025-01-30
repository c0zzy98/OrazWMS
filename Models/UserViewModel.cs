namespace OrazWMS.Models
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty; // ID użytkownika jako string (zgodnie z bazą)
        public string Name { get; set; } = string.Empty; // Nazwa użytkownika
        public string Email { get; set; } = string.Empty; // Email/Login użytkownika
        public string Phone { get; set; } = string.Empty; // Numer telefonu użytkownika
        public string Role { get; set; } = "Brak roli"; // Domyślnie "Brak roli"
        public DateTime CreatedAt { get; set; } // Data utworzenia użytkownika
    }
}
