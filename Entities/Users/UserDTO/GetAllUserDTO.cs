namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class GetAllUserDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }

        public string? Licence { get; set; }

        public string Email { get; set; }

        public List<Address>? Addresses { get; set; }
        public List<PhoneNumber>? PhoneNumbers { get; set; }
        public Role Role { get; set; } 
    }
}
