namespace DriveX_Backend.Helpers
{
    public class NICValidator
    {
        public static bool IsValidNIC(string nic)
        {
            if (nic.Length == 9)
            {

                char tenthCharacter = nic[8];
                return char.ToLower(tenthCharacter) == 'v' || char.ToLower(tenthCharacter) == 'u';
            }
            else if (nic.Length == 12)
            {
   
                return nic.All(char.IsDigit);
            }


            return false;
        }
    }
}
