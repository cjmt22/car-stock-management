namespace CarStock.API.DTOs
{
    // What the client sends when CREATING a car (POST)
    // Notice: no Id, no DealerId, no CreatedAt
    // The server fills those in automatically
    public class CreateCarDto
    {
        public string Make { get; set; } = string.Empty;        // e.g. "Toyota"
        public string Model { get; set; } = string.Empty;       // e.g. "Camry"
        public int Year { get; set; }                           // e.g. 2022
        public decimal Price { get; set; }                      // e.g. 25000.00
        public int Mileage { get; set; }                        // e.g. 15000
        public string Colour { get; set; } = string.Empty;      // e.g. "Silver"
        public string Description {  get; set; } = string.Empty;
    }

    // What the client sends when UPDATING a car (PUT)
    // ALl fields are nullable - client only sends what they want to change
    public class UpdateCarDtoP
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year {  get; set; }
        public decimal? Price { get; set; }
        public int? Mileage { get; set; }
        public string? Colour { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }       // "Available", "Reserved", "Sold"
    }

    // What the server sends back when returning a car
    // Contains everything the frontend needs to display
    public class CarResponseDto
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Mileage { get; set; }
        public string Colour { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Dealer info - flattened so frontend doesn't need to dig into nested objects
        public int DealerId { get; set; }
        public string DealerName { get; set; } = string.Empty;
    }

    // What the server returns for paginated lists
    // This is the wrapper around a page of results
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new(); // the actual data
        public int TotalCount { get; set; }         // total records in DB
        public int Page { get; set; }               // current page number
        public int PageSize { get; set; }           // items per page
        public int TotalPages { get; set; }         // how many pages total
    }

    // Query parameters for filtering and searching
    // Client sends: GET /api/cars?make=Toyota&maxPrice=30000&page=1
    public class CarQueryDto
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Colour { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
