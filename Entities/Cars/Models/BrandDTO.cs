namespace DriveX_Backend.Entities.Cars.Models
{
    public class BrandDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class BrandRequestDTO
    {
        public string Name { get; set; }
    }
}
