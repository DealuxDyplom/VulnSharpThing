using Microsoft.EntityFrameworkCore;
using VulnerableApp.Models;

namespace VulnerableApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уязвимость: Слабые пароли по умолчанию
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123", // Уязвимость: Слабый пароль
                    Email = "admin@test.com",
                    Role = "Admin",
                    CreditCard = "1234-5678-9012-3456"
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    Password = "password", // Уязвимость: Очень слабый пароль
                    Email = "user@test.com",
                    Role = "User",
                    CreditCard = "9876-5432-1098-7654"
                }
            );
        }
    }
}