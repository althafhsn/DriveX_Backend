namespace DriveX_Backend.Entities.Cars
{
    public class Car
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public Guid ModelId { get; set; }
        public Model Model { get; set; }
        public string RegNo { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<CarImage> Images { get; set; }
        public string? Action { get; set; }
        public string Status { get; set; }
        public decimal OngoingRevenue { get; set; }
        public decimal TotalRevenue { get; set; }

    }

}
