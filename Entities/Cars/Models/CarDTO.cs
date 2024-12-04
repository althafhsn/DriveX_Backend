namespace DriveX_Backend.Entities.Cars.Models
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime? StartDate { get; set; }   
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public string? RentalRequestStatus  { get; set; }
        public decimal? OngoingRevenue { get; set; }
        public decimal? TotalRevenue { get; set; }
    }

    public class CarRequestDTO
    {
        public Guid BrandId { get; set; }
        public Guid ModelId { get; set; }
        public string RegNo { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageRequestDTO> Images { get; set; }
    }

    public class CarCustomerDTO
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; }
        public string RegNo { get; set; }
        public decimal PricePerDay { get; set; }
        public string GearType { get; set; }
        public string FuelType { get; set; }
        public string Mileage { get; set; }
        public string SeatCount { get; set; }
        public List<ImageDTO> Images { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public int Duration { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? Year {  get; set; }

    }
}
