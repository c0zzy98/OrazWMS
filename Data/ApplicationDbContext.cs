using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Models; // ✅ IMPORT MODELI (w tym ActivityLog)

namespace OrazWMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!; // ✅ TERAZ DZIAŁA

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

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
}
