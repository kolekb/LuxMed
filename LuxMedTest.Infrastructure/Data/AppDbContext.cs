using LuxMedTest.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LuxMedTest.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja tabeli Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Password).IsRequired();

                // Indeks na kolumnie Username dla unikalności
                entity.HasIndex(u => u.Username).IsUnique();
            });

        }
    }
}
