namespace DriveX_Backend.Entities.Users
{
    public class Address
    {
        public Guid Id { get; set; }
        public string HouseNo { get; set; }
        public string Street1 { get; set; }
        public string? Street2 { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }

    }
}
