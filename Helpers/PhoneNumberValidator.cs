using System.Text.RegularExpressions;

namespace DriveX_Backend.Helpers
{
    public class PhoneNumberValidator
    {
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            // Regex for +94xxxxxxxxx format (12 digits)
            string format12Digits = @"^\+94\d{9}$";
            // Regex for numbers with 9-12 digits
            string generalFormat = @"^\d{9,12}$";

            // Check if phone number matches either condition
            if (Regex.IsMatch(phoneNumber, format12Digits))
            {
                return true;
            }
            else if (Regex.IsMatch(phoneNumber, generalFormat) && phoneNumber.Length <= 12)
            {
                return true;
            }

            return false;
        }
    }
}
