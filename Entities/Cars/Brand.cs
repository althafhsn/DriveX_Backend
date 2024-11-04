namespace DriveX_Backend.Entities.Cars
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Model> Models { get; set; }
    }
}
