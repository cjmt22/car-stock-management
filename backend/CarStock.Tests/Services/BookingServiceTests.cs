using CarStock.API.DTOs;
using CarStock.API.Models;
using CarStock.API.Services;
using CarStock.Tests.Helpers;
using FluentAssertions;

namespace CarStock.Tests.Services
{
    public class BookingServiceTests
    {
        // ─────────────────────────────────────────
        // CreateBooking Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task CreateBooking_WithValidData_ReturnsBooking()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new BookingService(db);

            var dto = new CreateBookingDto
            {
                CarId = 1,
                BookingDate = DateTime.UtcNow.AddDays(7),  // 1 week from now
                Notes = "Saturday morning please"
            };

            // Act — Buyer (Id=3) books car
            var (booking, error) = await service.CreateBookingAsync(dto, buyerId: 3);

            // Assert
            error.Should().BeNull();
            booking.Should().NotBeNull();
            booking!.Status.Should().Be("Pending");
            booking.CarMake.Should().Be("Toyota");
            booking.BuyerName.Should().Be("Test Buyer");
        }

        [Fact]
        public async Task CreateBooking_WithPastDate_ReturnsError()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);
            var service = new BookingService(db);

            var dto = new CreateBookingDto
            {
                CarId = 1,
                BookingDate = DateTime.UtcNow.AddDays(-1),  // yesterday — invalid
                Notes = "Test"
            };

            // Act
            var (booking, error) = await service.CreateBookingAsync(dto, buyerId: 3);

            // Assert
            booking.Should().BeNull();
            error.Should().Be("Booking date must be in the future.");
        }

        [Fact]
        public async Task CreateBooking_ForSoldCar_ReturnsError()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);

            // Mark car as Sold
            var car = db.Cars.Find(1);
            car!.Status = CarStatus.Sold;
            await db.SaveChangesAsync();

            var service = new BookingService(db);

            var dto = new CreateBookingDto
            {
                CarId = 1,
                BookingDate = DateTime.UtcNow.AddDays(7),
                Notes = "Test"
            };

            // Act
            var (booking, error) = await service.CreateBookingAsync(dto, buyerId: 3);

            // Assert
            booking.Should().BeNull();
            error.Should().Contain("Sold");
        }

        [Fact]
        public async Task CreateBooking_ForNonExistentCar_ReturnsError()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            var service = new BookingService(db);

            var dto = new CreateBookingDto
            {
                CarId = 999,  // doesn't exist
                BookingDate = DateTime.UtcNow.AddDays(7),
                Notes = "Test"
            };

            // Act
            var (booking, error) = await service.CreateBookingAsync(dto, buyerId: 3);

            // Assert
            booking.Should().BeNull();
            error.Should().Be("Car not found.");
        }

        // ─────────────────────────────────────────
        // UpdateBookingStatus Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task UpdateBookingStatus_DealerApproves_ReturnsApproved()
        {
            // Arrange — create a pending booking first
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);

            db.Bookings.Add(new Booking
            {
                Id = 1,
                CarId = 1,
                BuyerId = 3,
                BookingDate = DateTime.UtcNow.AddDays(7),
                Status = BookingStatus.Pending
            });
            await db.SaveChangesAsync();

            var service = new BookingService(db);
            var dto = new UpdateBookingStatusDto { Status = "Approved" };

            // Act — Dealer (Id=2) approves
            var (booking, error) = await service
                .UpdateBookingStatusAsync(1, dto, userId: 2, userRole: "Dealer");

            // Assert
            error.Should().BeNull();
            booking!.Status.Should().Be("Approved");
        }

        [Fact]
        public async Task UpdateBookingStatus_BuyerCancels_ReturnsCancelled()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);

            db.Bookings.Add(new Booking
            {
                Id = 1,
                CarId = 1,
                BuyerId = 3,
                BookingDate = DateTime.UtcNow.AddDays(7),
                Status = BookingStatus.Pending
            });
            await db.SaveChangesAsync();

            var service = new BookingService(db);
            var dto = new UpdateBookingStatusDto { Status = "Cancelled" };

            // Act — Buyer (Id=3) cancels their own booking
            var (booking, error) = await service
                .UpdateBookingStatusAsync(1, dto, userId: 3, userRole: "Buyer");

            // Assert
            error.Should().BeNull();
            booking!.Status.Should().Be("Cancelled");
        }

        [Fact]
        public async Task UpdateBookingStatus_BuyerTriesToApprove_ReturnsError()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            await TestDataSeeder.SeedCarsAsync(db);

            db.Bookings.Add(new Booking
            {
                Id = 1,
                CarId = 1,
                BuyerId = 3,
                BookingDate = DateTime.UtcNow.AddDays(7),
                Status = BookingStatus.Pending
            });
            await db.SaveChangesAsync();

            var service = new BookingService(db);
            var dto = new UpdateBookingStatusDto { Status = "Approved" };

            // Act — Buyer tries to approve (not allowed)
            var (booking, error) = await service
                .UpdateBookingStatusAsync(1, dto, userId: 3, userRole: "Buyer");

            // Assert
            booking.Should().BeNull();
            error.Should().Be("Buyers can only cancel bookings.");
        }
    }
}