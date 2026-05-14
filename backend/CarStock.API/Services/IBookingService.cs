using CarStock.API.DTOs;

namespace CarStock.API.Services
{
    public interface IBookingService
    {
        // Buyer creates a booking
        Task<(BookingResponseDto? booking, string? error)> CreateBookingAsync(
            CreateBookingDto dto, int buyerId);

        // Buyer gets their own bookings
        Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(int buyerId);

        // Admin gets ALL bookings across the platform
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();

        // Dealer gets bookings only for their own cars
        Task<IEnumerable<BookingResponseDto>> GetDealerBookingsAsync(int dealerId);

        // Dealer or Admin updates a booking status (Approve/Reject)
        // Buyer can cancel their own booking
        Task<(BookingResponseDto? booking, string? error)> UpdateBookingStatusAsync(
            int bookingId, UpdateBookingStatusDto dto, int userId, string userRole);
    }
}
