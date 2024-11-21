namespace DriveX_Backend.Entities.Cars.Models
{
    public class UpdateCarDTO
    {
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
    }
}
