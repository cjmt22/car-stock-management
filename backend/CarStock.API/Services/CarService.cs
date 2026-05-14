using CarStock.API.Data;
using CarStock.API.DTOs;
using CarStock.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CarStock.API.Services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _db;

        public CarService(AppDbContext db)
        {
            _db = db;
        }

        // Helper: converts a Car entity into a CarResponseDto
        // We use this in every method so we don't repeat ourselves (DRY principle)
        private static CarResponseDto ToDto(Car car) => new()
        {
            Id = car.Id,
            Make = car.Make,
            Model = car.Model,
            Year = car.Year,
            Price = car.Price,
            Mileage = car.Mileage,
            Colour = car.Colour,
            Description = car.Description,
            ImageUrl = car.ImageUrl,
            Status = car.Status.ToString(),
            CreatedAt = car.CreatedAt,
            DealerId = car.DealerId,
            // Dealer name comes from the navigation property (joined from Users table)
            DealerName = car.Dealer?.Name ?? "Unknown"
        };

        public async Task<PagedResultDto<CarResponseDto>> GetAllCarsAsync(CarQueryDto query)
        {
            // Start with all cars, include the Dealer navigation property
            // Include() tells EF Core to JOIN the Users table so we get dealer info
            var q = _db.Cars
                .Include(c => c.Dealer)
                .AsQueryable();

            // Apply filters only if the client sent them
            // This builds a dynamic WHERE clause
            if (!string.IsNullOrEmpty(query.Make))
                q = q.Where(c => c.Make.ToLower().Contains(query.Make.ToLower()));

            if (!string.IsNullOrEmpty(query.Model))
                q = q.Where(c => c.Model.ToLower().Contains(query.Model.ToLower()));

            if (query.MinYear.HasValue)
                q = q.Where(c => c.Year >= query.MinYear.Value);

            if (query.MaxYear.HasValue)
                q = q.Where(c => c.Year <= query.MaxYear.Value);

            if (query.MinPrice.HasValue)
                q = q.Where(c => c.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                q = q.Where(c => c.Price <= query.MaxPrice.Value);

            if (!string.IsNullOrEmpty(query.Status) &&
                Enum.TryParse<CarStatus>(query.Status, true, out var status))
                q = q.Where(c => c.Status == status);

            // Count BEFORE pagination - this is the total matching records
            var totalCount = await q.CountAsync();

            // Apply pagination
            // Skip() jumps past previous pages, Take() grabs the current page
            // e.g. Page 2, PageSize 10: Skip(10).Take(10) = records 11-20
            var items = await q
                .OrderByDescending(c => c.CreatedAt) // newest first
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => ToDto(c))
                .ToListAsync();

            return new PagedResultDto<CarResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }

        public async Task<CarResponseDto?> GetCarByIdAsync(int id)
        {
            var car = await _db.Cars
                .Include(c => c.Dealer)
                .FirstOrDefaultAsync(c => c.Id == id);

            return car is null ? null : ToDto(car);
        }

        public async Task<CarResponseDto> CreateCarAsync(CreateCarDto dto, int dealerId)
        {
            var car = new Car
            {
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Price = dto.Price,
                Mileage = dto.Mileage,
                Colour = dto.Colour,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                DealerId = dealerId,         // taken from JWT token - not from the request body
                Status = CarStatus.Available // always starts as Available
            };

            _db.Cars.Add(car);
            await _db.SaveChangesAsync();

            // Reload with Dealer info so DealerName is populated in the response
            await _db.Entry(car).Reference(c => c.Dealer).LoadAsync();

            return ToDto(car);
        }

        public async Task<CarResponseDto?> UpdateCarAsync(
            int id, UpdateCarDtoP dto, int userId, string userRole)
        {
            var car = await _db.Cars
                .Include(c => c.Dealer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car is null) return null;

            // Ownership check - Dealers can only update their own cars
            // Admins can update any car
            if (userRole != "Admin" && car.DealerId != userId)
                return null; // controller will return 403 Forbidden

            // Only update fields that were actually sent
            // This is a PATCH-style update inside a PUT endpoint
            if (dto.Make is not null) car.Make = dto.Make;
            if (dto.Model is not null) car.Model = dto.Model;
            if (dto.Year.HasValue) car.Year = dto.Year.Value;
            if (dto.Price.HasValue) car.Price = dto.Price.Value;
            if (dto.Mileage.HasValue) car.Mileage = dto.Mileage.Value;
            if (dto.Colour is not null) car.Colour = dto.Colour;
            if (dto.Description is not null) car.Description = dto.Description;
            if (dto.ImageUrl is not null) car.ImageUrl = dto.ImageUrl;

            // Parse and validate the status string if provided
            if (dto.Status is not null &&
                Enum.TryParse<CarStatus>(dto.Status, true, out var newStatus))
                car.Status = newStatus;

            await _db.SaveChangesAsync();
            return ToDto(car);
        }

        public async Task<bool> DeleteCarAsync(int id, int userId, string userRole)
        {
            var car = await _db.Cars.FindAsync(id);

            if (car is null) return false;

            // Ownership check - same logic as update
            if (userRole != "Admin" && car.DealerId != userId)
                return false; // controller will return 403 forbidden

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
