namespace DriveX_Backend.Entities.Cars
{
    public class CarImage
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public Guid CarId { get; set; }
        public Car Car { get; set; }
    }
}
