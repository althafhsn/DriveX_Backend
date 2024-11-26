namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class DashboardAllCustomerDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Image { get; set; }
        public List<PhoneNumber> PhoneNumber { get; set; }
        public string Email { get; set; }
        public string status { get; set; }
    }
}
