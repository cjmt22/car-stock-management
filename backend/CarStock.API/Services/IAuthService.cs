using CarStock.API.DTOs;

namespace CarStock.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
    }
}
