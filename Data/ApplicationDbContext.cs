using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Models;

namespace OrazWMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<OrazWMS.Models.ApplicationUser> // Poprawne dziedziczenie
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!; // Logi aktywności

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja ApplicationUser
            modelBuilder.Entity<OrazWMS.Models.ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Domyślna wartość daty utworzenia

            // Konfiguracja ActivityLog
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Action)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(a => a.Timestamp)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });
        }
    }

    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data utworzenia konta
    }

    public class ActivityLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public OrazWMS.Models.ApplicationUser User { get; set; } = null!;
    }
}
