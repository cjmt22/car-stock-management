using CarStock.API.DTOs;

namespace CarStock.API.Services
{
    public interface ICarService
    {
        // Get all cars with optional filters and pagination
        Task<PagedResultDto<CarResponseDto>> GetAllCarsAsync(CarQueryDto query);

        // Get one car by its Id
        Task<CarResponseDto?> GetCarByIdAsync(int id);

        // Create a new car - dealerId comes from the JWT token
        Task<CarResponseDto> CreateCarAsync(CreateCarDto dto, int dealerId);

        // Update a car - returns null if not found or not owned by this dealer
        Task<CarResponseDto?> UpdateCarAsync(int id, UpdateCarDtoP dto, int userId, string userRole);

        // Delete a car - returns false if not found or not owned by this dealer
        Task<bool> DeleteCarAsync(int id, int userId, string userRole);
    }
}
