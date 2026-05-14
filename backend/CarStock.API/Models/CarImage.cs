namespace CarStock.API.Models
{
    public class CarImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

    }
}
