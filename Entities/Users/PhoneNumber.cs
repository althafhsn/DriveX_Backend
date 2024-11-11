namespace DriveX_Backend.Entities.Users
{
    public class PhoneNumber
    {
        public Guid Id { get; set; }
        public string Mobile1 { get; set; }
        public string? Mobile2 { get; set; }
        public Guid UserId { get; set; }
    }
}
