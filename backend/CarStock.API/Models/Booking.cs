namespace CarStock.API.Models
{
    public enum BookingStatus { Pending, Approved, Rejected, Cancelled }

    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public int BuyerId { get; set; }
        public User Buyer { get; set; } = null!;
    }
}
