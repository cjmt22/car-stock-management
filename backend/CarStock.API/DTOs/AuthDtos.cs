namespace CarStock.API.DTOs
{

    // What the client sends when registering
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        // Role defaults to Buyer - only Admin can create Dealers via AdminController
    }

    // What the client sends when logging in
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // What the server sends back after successful login
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    // What the server sends back for GET /me
    public class CurrentUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}
