using CarStock.API.DTOs;
using CarStock.API.Models;
using CarStock.API.Services;
using CarStock.Tests.Helpers;
using FluentAssertions;

namespace CarStock.Tests.Services
{
    public class CarServiceTests
    {
        // ─────────────────────────────────────────
        // GetAllCars Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task GetAllCars_ReturnsPagedResult()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            var query = new CarQueryDto { Page = 1, PageSize = 10 };

            // Act
            var result = await service.GetAllCarsAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);    // we seeded 2 cars
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetAllCars_FilterByMake_ReturnsMatchingCars()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Filter for only Toyota
            var query = new CarQueryDto
            {
                Make = "Toyota",
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await service.GetAllCarsAsync(query);

            // Assert — only 1 Toyota in seeded data
            result.Items.Should().HaveCount(1);
            result.Items.First().Make.Should().Be("Toyota");
        }

        [Fact]
        public async Task GetAllCars_FilterByMaxPrice_ReturnsAffordableCars()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Max price 25000 — Honda (22000) passes, Toyota (28000) doesn't
            var query = new CarQueryDto
            {
                MaxPrice = 25000,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = await service.GetAllCarsAsync(query);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().Make.Should().Be("Honda");
        }

        // ─────────────────────────────────────────
        // GetCarById Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task GetCarById_WithValidId_ReturnsCar()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Act
            var result = await service.GetCarByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Make.Should().Be("Toyota");
            result.Model.Should().Be("Camry");
        }

        [Fact]
        public async Task GetCarById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            var service = new CarService(db);

            // Act
            var result = await service.GetCarByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        // ─────────────────────────────────────────
        // CreateCar Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task CreateCar_WithValidData_ReturnsCar()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            var service = new CarService(db);

            var dto = new CreateCarDto
            {
                Make = "Mazda",
                Model = "CX-5",
                Year = 2023,
                Price = 35000,
                Mileage = 8000,
                Colour = "Red",
                Description = "Near new"
            };

            // Act — dealerId 2 is our test Dealer
            var result = await service.CreateCarAsync(dto, dealerId: 2);

            // Assert
            result.Should().NotBeNull();
            result.Make.Should().Be("Mazda");
            result.Status.Should().Be("Available");
            result.DealerId.Should().Be(2);
        }

        // ─────────────────────────────────────────
        // DeleteCar Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task DeleteCar_ByOwner_ReturnsTrue()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Act — Dealer (Id=2) deletes their own car (Id=1)
            var result = await service.DeleteCarAsync(id: 1, userId: 2, userRole: "Dealer");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCar_ByNonOwner_ReturnsFalse()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Act — Buyer (Id=3) tries to delete Dealer's car
            var result = await service.DeleteCarAsync(id: 1, userId: 3, userRole: "Buyer");

            // Assert — should be blocked by ownership check
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteCar_ByAdmin_ReturnsTrue()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new CarService(db);

            // Act — Admin can delete any car
            var result = await service.DeleteCarAsync(id: 1, userId: 1, userRole: "Admin");

            // Assert
            result.Should().BeTrue();
        }
    }
}