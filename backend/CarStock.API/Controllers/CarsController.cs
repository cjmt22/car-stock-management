using CarStock.API.DTOs;
using CarStock.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace CarStock.API.Controllers
{
    [ApiController]
    [Route("api/cars")]
    public class CarsController : ControllerBase 
    {
        private readonly ICarService _carService;
        private readonly IAuditService _auditService;

        public CarsController(ICarService carService, IAuditService auditService)
        {
            _carService = carService;
            _auditService = auditService;
        }

        // ----------------------------------------------------------------------
        // GET /api/cars
        // Optional query params: ?make=Toyota&maxPrice=30000&page=1&pageeSize=10
        // No auth required - anyone can browse cars
        // ----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllCars([FromQuery] CarQueryDto query)
        {
            // Validate page siez - prevent someone requesting 10000 items at once
            if (query.PageSize > 50) query.PageSize = 50;
            if (query.PageSize < 1) query.PageSize = 10;
            if (query.Page < 1) query.Page = 1;

            var result = await _carService.GetAllCarsAsync(query);
            return Ok(result);
        }

        // -------------------
        // GET /api/cars/{id}
        // No auth required
        // -------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);

            if (car is null)
                return NotFound(new { message = $"Car with Id {id} was not found." });

            return Ok(car);
        }

        // -------------------------------------------
        // POST /api/cars
        // Only Dealers and Admins can create listings
        // -------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Dealer,Admin")]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarDto dto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.Make) ||
                string.IsNullOrWhiteSpace(dto.Model))
                return BadRequest(new { message = "Make and Model are required." });

            if (dto.Year < 1900 || dto.Year > DateTime.Now.Year + 1)
                return BadRequest(new { message = "Please enter a valid year." });

            if (dto.Price <= 0)
                return BadRequest(new { message = "Price must be greater than zero." });

            // Get the dealer's userId from their JWT token
            var dealerId = GetCurrentUserId();

            var car = await _carService.CreateCarAsync(dto, dealerId);

            // Log to audit trail
            await _auditService.LogAsync(dealerId, "CreateCar", "Car", car.Id);

            // 201 Created — includes a Location header pointing to GET /api/cars/{id}
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        // --------------------------------------------------------
        // PUT /api/cars/{id}
        // Dealers can update their own cars, Admins can update any
        // --------------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Dealer,Admin")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarDtoP dto)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var updated = await _carService.UpdateCarAsync(id, dto, userId, userRole);

            if (updated is null)
            {
                // Could be not found OR not owned by this dealer
                // We return 403 if the car exists but doesn't belong to them
                var exists = await _carService.GetCarByIdAsync(id);
                if (exists is null)
                    return NotFound(new { message = $"Car with Id {id} was not found." });

                return StatusCode(403, new { message = "You can only update your own listings." });
            }

            await _auditService.LogAsync(userId, "UpdateCar", "Car", id);
            return Ok(updated);

        }

        // --------------------------------------------------------
        // DELETE /api/cars/{id}
        // Dealers can delete their own cars, Admins can delete any
        // --------------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Dealer,Admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var deleted = await _carService.DeleteCarAsync(id, userId, userRole);

            if (!deleted)
            {
                var exists = await _carService.GetCarByIdAsync(id);
                if (exists is null)
                    return NotFound(new { message = $"Car with Id {id} was not found." });

                return StatusCode(403, new { message = "You can only delete your own listings." });
            }

            await _auditService.LogAsync(userId, "DeleteCar", "Car", id);
            return NoContent();  // 204 — success, nothing to return
        }

        // --------------------------------------------
        // Private helpers - read claims from JWT token
        // --------------------------------------------
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
