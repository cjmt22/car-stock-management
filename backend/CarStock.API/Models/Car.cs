namespace CarStock.API.Models
{
    public enum CarStatus { Available, Reserved, Sold }

    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Mileage { get; set; }
        public string Colour { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CarStatus Status { get; set; } = CarStatus.Available;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int DealerId { get; set; }
        public User Dealer { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<CarImage> Images { get; set; } = new List<CarImage>();
    }
}
