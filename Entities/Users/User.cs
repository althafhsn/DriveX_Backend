namespace DriveX_Backend.Entities.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string? Image { get; set; } = "https://www.freepik.com/free-vector/young-prince-vector-illustration_354177187.htm#fromView=keyword&page=1&position=2&uuid=36b315b4-85fc-40a6-8e58-93c531499cf7";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NIC { get; set; }

        public string? Token { get; set; }
        public string? Licence { get; set; }
        
        public string Email { get; set; }

        public List<Address>? Addresses { get; set; }
        public List<PhoneNumber>? PhoneNumbers { get; set; }

        public string Password { get; set; }
        public Role Role { get; set; } = Role.Customer;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? ForgetPasswordToken { get; set; }
        public DateTime? ForgetPasswordTokenExpiry { get; set; }

        public string status { get; set; }
        public string Notes { get; set; }


    }
}
