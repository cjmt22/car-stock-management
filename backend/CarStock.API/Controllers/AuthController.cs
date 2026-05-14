using CarStock.API.DTOs;
using CarStock.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarStock.API.Controllers
{

    [ApiController]
    [Route("api/auth")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;

        public AuthController(IAuthService authService, IAuditService auditService)
        {
            _authService = authService;
            _auditService = auditService;
        }

        // --------------------------------------------
        // POST /api/auth/register
        // Anyone can call this - no [Authorize] needed
        // --------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Name, email and password are required." });

            if (dto.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters." });

            var result = await _authService.RegisterAsync(dto);

            // null means email already exists
            if (result == null)
                return Conflict(new { message = "An account with this email already exists." });

            // Log the registration
            await _auditService.LogAsync(0, "Register", "User", 0);

            return Ok(result); // 200 with token + user info

        }

        // --------------------------------------------
        // POST /api/auth/login
        // Anyone can call this - no [Authorize] needed
        // --------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email and password are required." });

            var result = await _authService.LoginAsync(dto);

            // null means wrong email or password
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result); // 200 with token + user info
        }

        // -----------------------------------------------------
        // GET /api/auth/me
        // Requires a valid JWT - returns current user's profile
        // -----------------------------------------------------
        [HttpGet("me")]
        [Authorize] // any logged-in user can call this
        public async Task<IActionResult> Me()
        {
            // Read the userId from the JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var user = await _authService.GetCurrentUserAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }
    }
}
