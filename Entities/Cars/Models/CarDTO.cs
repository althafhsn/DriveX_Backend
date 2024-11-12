namespace DriveX_Backend.Entities.Cars.Models
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public Guid ModelId { get; set; }
        public Model Model { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerHour { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
    }

    public class CarRequestDTO
    {
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerHour { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
    }
}
