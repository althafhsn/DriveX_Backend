namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class ProfileCustomerResponse
    {
        public Guid Id { get; set; }
        public string? Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Licence { get; set; }
    }
    public class ProfileCustomerRequest
    {
        public string? Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Licence { get; set; }
    }
}
