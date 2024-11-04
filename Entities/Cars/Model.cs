namespace DriveX_Backend.Entities.Cars
{
    public class Model
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
