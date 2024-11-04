namespace DriveX_Backend.Entities.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Image {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }
        public string Licence { get; set; }
        
        public string Email { get; set; }

        public List<Address> Addresses { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }

        public string Password { get; set; }
        public Role Role { get; set; }

    }
}
