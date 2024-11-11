namespace DriveX_Backend.Entities.Cars.Models
{
    public class ModelResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
    }

    public class ModelRequestDTO
    {
        public string Name { get; set; }
        public Guid BrandId { get; set; }
    }
}
