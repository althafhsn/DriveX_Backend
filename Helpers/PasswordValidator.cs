namespace DriveX_Backend.Helpers
{
    public class PasswordValidator
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
