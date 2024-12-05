using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OrazWMS.Data
{
    // Klasa ApplicationDbContext dziedziczy po IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet dla ActivityLog
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!; // Zainicjalizowane

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Wywołanie podstawowej konfiguracji dla Identity
            base.OnModelCreating(modelBuilder);

            // Konfiguracja tabeli ActivityLog
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(a => a.Id); // Klucz główny
                entity.Property(a => a.Action)
                      .IsRequired()
                      .HasMaxLength(255); // Wymagana właściwość z ograniczeniem długości
                entity.Property(a => a.Timestamp)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP") // Domyślna wartość w PostgreSQL
                      .IsRequired();
                entity.HasOne(a => a.User)
                      .WithMany() // Brak nawigacji w drugą stronę
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Usunięcie logu, gdy użytkownik zostanie usunięty
            });
        }
    }

    // Klasa ActivityLog dla logów aktywności
    public class ActivityLog
    {
        public int Id { get; set; } // Klucz główny
        public string Action { get; set; } = string.Empty; // Wymagana właściwość z wartością domyślną
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Domyślny czas
        public string UserId { get; set; } = string.Empty; // Klucz obcy do IdentityUser
        public IdentityUser User { get; set; } = null!; // Nawigacja do użytkownika
    }
}
