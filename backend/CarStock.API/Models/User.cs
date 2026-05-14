namespace CarStock.API.Models

{
    public enum UserRole { Admin, Dealer, Buyer }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Buyer;
        public bool isActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
