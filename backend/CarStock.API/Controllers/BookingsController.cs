using CarStock.API.DTOs;
using CarStock.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarStock.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]  // ALL endpoints require login — no anonymous access
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IAuditService _auditService;

        public BookingsController(
            IBookingService bookingService,
            IAuditService auditService)
        {
            _bookingService = bookingService;
            _auditService = auditService;
        }

        // ─────────────────────────────────────────
        // POST /api/bookings
        // Buyer books a test drive
        // ─────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (dto.CarId <= 0)
                return BadRequest(new { message = "A valid CarId is required." });

            if (dto.BookingDate == default)
                return BadRequest(new { message = "A valid booking date is required." });

            var buyerId = GetCurrentUserId();
            var (booking, error) = await _bookingService.CreateBookingAsync(dto, buyerId);

            if (error is not null)
                return BadRequest(new { message = error });

            await _auditService.LogAsync(buyerId, "CreateBooking", "Booking", booking!.Id);

            return CreatedAtAction(nameof(GetMyBookings), new { }, booking);
        }

        // ─────────────────────────────────────────
        // GET /api/bookings/my
        // Buyer sees their own bookings
        // ─────────────────────────────────────────
        [HttpGet("my")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetMyBookings()
        {
            var buyerId = GetCurrentUserId();
            var bookings = await _bookingService.GetMyBookingsAsync(buyerId);
            return Ok(bookings);
        }

        // ─────────────────────────────────────────
        // GET /api/bookings
        // Admin sees ALL bookings on the platform
        // ─────────────────────────────────────────
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        // ─────────────────────────────────────────
        // GET /api/bookings/dealer
        // Dealer sees bookings for their own cars only
        // ─────────────────────────────────────────
        [HttpGet("dealer")]
        [Authorize(Roles = "Dealer")]
        public async Task<IActionResult> GetDealerBookings()
        {
            var dealerId = GetCurrentUserId();
            var bookings = await _bookingService.GetDealerBookingsAsync(dealerId);
            return Ok(bookings);
        }

        // ─────────────────────────────────────────
        // PATCH /api/bookings/{id}/status
        // Dealer: Approve or Reject
        // Buyer: Cancel their own booking
        // Admin: can do anything
        // ─────────────────────────────────────────
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Buyer,Dealer,Admin")]
        public async Task<IActionResult> UpdateBookingStatus(
            int id, [FromBody] UpdateBookingStatusDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { message = "Status is required." });

            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var (booking, error) = await _bookingService
                .UpdateBookingStatusAsync(id, dto, userId, userRole);

            if (error is not null)
                return BadRequest(new { message = error });

            await _auditService.LogAsync(userId, $"UpdateBookingStatus:{dto.Status}",
                "Booking", id);

            return Ok(booking);
        }

        // ─────────────────────────────────────────
        // Private helpers — read JWT claims
        // ─────────────────────────────────────────
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}