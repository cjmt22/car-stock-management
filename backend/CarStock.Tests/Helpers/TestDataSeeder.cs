using CarStock.API.Data;
using CarStock.API.Models;

namespace CarStock.Tests.Helpers
{
    public static class TestDataSeeder
    {
        // Creates a standard set of test users
        public static async Task SeedUsersAsync(AppDbContext db)
        {
            db.Users.AddRange(
                new User
                {
                    Id = 1,
                    Name = "Test Admin",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = UserRole.Admin,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Name = "Test Dealer",
                    Email = "dealer@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = UserRole.Dealer,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Name = "Test Buyer",
                    Email = "buyer@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = UserRole.Buyer,
                    IsActive = true
                }
            );
            await db.SaveChangesAsync();
        }

        // Creates test cars owned by Dealer (Id=2)
        public static async Task SeedCarsAsync(AppDbContext db)
        {
            db.Cars.AddRange(
                new Car
                {
                    Id = 1,
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2022,
                    Price = 28000,
                    Mileage = 15000,
                    Colour = "Silver",
                    Description = "Test car",
                    Status = CarStatus.Available,
                    DealerId = 2
                },
                new Car
                {
                    Id = 2,
                    Make = "Honda",
                    Model = "Civic",
                    Year = 2021,
                    Price = 22000,
                    Mileage = 28000,
                    Colour = "Blue",
                    Description = "Test car 2",
                    Status = CarStatus.Available,
                    DealerId = 2
                }
            );
            await db.SaveChangesAsync();
        }
    }
}