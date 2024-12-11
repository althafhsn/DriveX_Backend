namespace DriveX_Backend.Helpers
{
    public class DateTimeValidator
    {
        public static DateTime GetSriLankanTime()
        {
            TimeZoneInfo sriLankaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
            DateTime sriLankaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sriLankaTimeZone);

            return new DateTime(
                sriLankaTime.Year,
                sriLankaTime.Month,
                sriLankaTime.Day,
                sriLankaTime.Hour,
                sriLankaTime.Minute,
                0 // Set seconds to 0
            );
        }
    }
}
