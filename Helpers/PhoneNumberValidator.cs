using System.Text.RegularExpressions;

namespace DriveX_Backend.Helpers
{
    public class PhoneNumberValidator
    {
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            // Regex for Sri Lankan phone numbers in +94xxxxxxxxx format
            string sriLankaFormat = @"^\+94\d{9}$";
            // Regex for general numbers with exactly 9 or 10 digits
            string generalFormat = @"^\d{9,10}$";

            // Validate against either format
            return Regex.IsMatch(phoneNumber, sriLankaFormat) || Regex.IsMatch(phoneNumber, generalFormat);
        }
    }
}
