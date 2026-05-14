namespace CarStock.API.DTOs
{
    // What the Buyer sends when booking a test drive
    public class CreateBookingDto
    {
        public int CarId { get; set; }
        public DateTime BookingDate { get; set; }  // preferred date/time
        public string Notes { get; set; } = string.Empty;  // any message to dealer
    }

    // What the Dealer/Admin sends when updating booking status
    public class UpdateBookingStatusDto
    {
        // Must be "Approved", "Rejected", or "Cancelled"
        public string Status { get; set; } = string.Empty;
    }

    // What the server returns when showing a booking
    // Contains full details — car info, buyer info, status
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Car details — flattened so frontend doesn't need nested objects
        public int CarId { get; set; }
        public string CarMake { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CarYear { get; set; }
        public decimal CarPrice { get; set; }

        // Buyer details
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
    }
}
