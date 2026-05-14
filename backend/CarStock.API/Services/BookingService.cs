using CarStock.API.Data;
using CarStock.API.DTOs;
using CarStock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarStock.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;

        public BookingService(AppDbContext db)
        {
            _db = db;
        }

        // Helper: converts Booking entity → BookingResponseDto
        // We reuse this in every method (DRY principle)
        private static BookingResponseDto ToDto(Booking b) => new()
        {
            Id = b.Id,
            BookingDate = b.BookingDate,
            Status = b.Status.ToString(),
            Notes = b.Notes,
            CreatedAt = b.CreatedAt,
            CarId = b.CarId,
            // Car navigation property gives us car details
            CarMake = b.Car?.Make ?? string.Empty,
            CarModel = b.Car?.Model ?? string.Empty,
            CarYear = b.Car?.Year ?? 0,
            CarPrice = b.Car?.Price ?? 0,
            BuyerId = b.BuyerId,
            // Buyer navigation property gives us buyer details
            BuyerName = b.Buyer?.Name ?? string.Empty,
            BuyerEmail = b.Buyer?.Email ?? string.Empty
        };

        public async Task<(BookingResponseDto? booking, string? error)>
            CreateBookingAsync(CreateBookingDto dto, int buyerId)
        {
            // Rule 1: The car must exist
            var car = await _db.Cars.FindAsync(dto.CarId);
            if (car is null)
                return (null, "Car not found.");

            // Rule 2: Car must be Available — can't book a Sold or Reserved car
            if (car.Status != CarStatus.Available)
                return (null, $"This car is currently {car.Status} and cannot be booked.");

            // Rule 3: Booking date must be in the future
            if (dto.BookingDate <= DateTime.UtcNow)
                return (null, "Booking date must be in the future.");

            // Rule 4: Buyer can't already have a Pending or Approved booking for same car
            var existingBooking = await _db.Bookings
                .AnyAsync(b => b.CarId == dto.CarId
                    && b.BuyerId == buyerId
                    && (b.Status == BookingStatus.Pending
                        || b.Status == BookingStatus.Approved));

            if (existingBooking)
                return (null, "You already have an active booking for this car.");

            var booking = new Booking
            {
                CarId = dto.CarId,
                BuyerId = buyerId,
                BookingDate = DateTime.SpecifyKind(dto.BookingDate, DateTimeKind.Utc),
                Notes = dto.Notes,
                Status = BookingStatus.Pending
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            // Reload with navigation properties so response has full details
            await _db.Entry(booking).Reference(b => b.Car).LoadAsync();
            await _db.Entry(booking).Reference(b => b.Buyer).LoadAsync();

            return (ToDto(booking), null);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int buyerId)
        {
            return await _db.Bookings
                .Include(b => b.Car)
                .Include(b => b.Buyer)
                .Where(b => b.BuyerId == buyerId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            return await _db.Bookings
                .Include(b => b.Car)
                .Include(b => b.Buyer)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetDealerBookingsAsync(int dealerId)
        {
            // Only bookings where the car belongs to this dealer
            return await _db.Bookings
                .Include(b => b.Car)
                .Include(b => b.Buyer)
                .Where(b => b.Car.DealerId == dealerId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<(BookingResponseDto? booking, string? error)>
            UpdateBookingStatusAsync(
                int bookingId, UpdateBookingStatusDto dto,
                int userId, string userRole)
        {
            // Load booking with car info
            var booking = await _db.Bookings
                .Include(b => b.Car)
                .Include(b => b.Buyer)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null)
                return (null, "Booking not found.");

            // Parse the new status string into the enum
            if (!Enum.TryParse<BookingStatus>(dto.Status, true, out var newStatus))
                return (null, "Invalid status. Use: Approved, Rejected, or Cancelled.");

            // Business rules for who can set what status:

            // Buyers can only Cancel their own bookings
            if (userRole == "Buyer")
            {
                if (booking.BuyerId != userId)
                    return (null, "You can only cancel your own bookings.");

                if (newStatus != BookingStatus.Cancelled)
                    return (null, "Buyers can only cancel bookings.");

                if (booking.Status == BookingStatus.Cancelled)
                    return (null, "This booking is already cancelled.");
            }

            // Dealers can only Approve or Reject bookings for their own cars
            if (userRole == "Dealer")
            {
                if (booking.Car.DealerId != userId)
                    return (null, "You can only manage bookings for your own cars.");

                if (newStatus == BookingStatus.Cancelled)
                    return (null, "Dealers cannot cancel bookings. Use Rejected instead.");

                if (booking.Status != BookingStatus.Pending)
                    return (null, "You can only update Pending bookings.");
            }

            // Rule: Can't change a Cancelled or Rejected booking
            if (userRole == "Admin")
            {
                if (booking.Status == BookingStatus.Cancelled
                    || booking.Status == BookingStatus.Rejected)
                    return (null, "Cannot update a closed booking.");
            }

            booking.Status = newStatus;
            await _db.SaveChangesAsync();

            return (ToDto(booking), null);
        }
    }
}