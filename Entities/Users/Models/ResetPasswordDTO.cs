namespace DriveX_Backend.Entities.Users.Models
{
    public record ResetPasswordDTO
    {

        public string Email { get; set; }
        public string EmailToken { get; set; }

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
