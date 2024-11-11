namespace DriveX_Backend.Entities.Users.Models
{
    public class Signup
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public Role Role { get; set; } = 0;


    }
    public class SignupRequest : Signup
    {
        
        public string Password { get; set; }
    }

    public class SignUpResponse : Signup
    {
        public Guid Id { get; set; }
    }

}
