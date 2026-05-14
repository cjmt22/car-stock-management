using CarStock.API.Data;
using CarStock.API.DTOs;
using CarStock.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarStock.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        // IConfiguration lets us read appsettings.json values (JWT key, issuer, etc.)
        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // 1. Check if email is already taken
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (existingUser != null)
                return null; // Email already exists - controller will return 409 Conflict

            // 2. Hash the password using BCrypt
            // BCrypt automatically adds a "salt" so two users with the same
            // password will have different hashes - very secure
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Create the new user
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email.ToLower(),
                PasswordHash = passwordHash,
                Role = UserRole.Buyer // all self-registrations are Buyers
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // 4. Generate and return a JWT token immediately
            // (so they're logged in right after registering)
            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // 1. Find the user by email
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            // 2. If not found OR account is inactive, return null
            if (user == null || !user.isActive)
                return null;

            // 3. Verify password against the stored hash
            // BCrypt.Verify compares the plain password to the hash safely
            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
                return null;

            // 4. Password correct - generate and return JWT token
            return GenerateAuthResponse(user);

        } 

        public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return null;

            return new CurrentUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        // -------------------------------------
        // Private helper - builds the JWT token
        // This is the core of authentication
        // -------------------------------------
        private AuthResponseDto GenerateAuthResponse(User user)
        {
            // Read JWT settings from appsettings.json
            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiresInMinutes = int.Parse(_config["Jwt:ExpiresInMinutes"]!);

            // Claims are pieces of information embedded inside the token
            // Anyone with the token can READ these (they're base64 encoded, not encrypted)
            // But they CANNOT fake or modify them - the signature protects that
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Create the signing key from your secret in appsettings.json
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            // Build the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            // Serialize the token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = expiry
            };
        }

    }
}
