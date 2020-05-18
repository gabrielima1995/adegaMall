namespace System
{
    public static class DateTimeHelper
    {
        public static string ISO_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

        public static string ToIsoString(this DateTime date)
        {
            return date.ToString(ISO_FORMAT);
        }

        public static string ToIsoDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}