namespace DriveX_Backend.Entities.Users.UserDTO
{
    public class CustomerResponseDto
    {
        public Guid Id { get; set; }
        public string? Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }
        public string? Licence { get; set; }
        public string Email { get; set; }
        public List<AddressDTO>? Addresses { get; set; }
        public List<PhoneNumberDTO>? PhoneNumbers { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
