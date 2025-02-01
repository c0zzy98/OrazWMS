namespace OrazWMS.Models
{
    public class ActivityLog
    {
        public int Id { get; set; } // Klucz główny
        public string Action { get; set; } = string.Empty; // Opis akcji
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Data zdarzenia
        public string UserId { get; set; } = string.Empty; // Klucz użytkownika
        public ApplicationUser User { get; set; } = null!; // Powiązanie z użytkownikiem
    }
}
