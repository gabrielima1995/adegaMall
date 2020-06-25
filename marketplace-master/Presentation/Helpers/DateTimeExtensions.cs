using Framework;
using Microsoft.AspNet.Identity;
using System.Web;

namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToDayAndMonthString(this DateTime dateTime)
        {
            return string.Format("{0:dd/MM}", dateTime);
        }

        private static DateTime ConvertTimeToTimeZone(DateTime date, string timeZoneId)
        {
            if (timeZoneId == null && HttpContext.Current.Request.IsAuthenticated)
            {
                var loginProfile = HttpContext.Current.User.Identity.GetLoginProfile();
                timeZoneId = loginProfile.PreferredTimeZoneId;
            }

            var timeZone = !string.IsNullOrEmpty(timeZoneId)
                ? TimeZones.Find(timeZoneId)
                : TimeZones.Default;

            if (date.Kind == DateTimeKind.Local)
                date = date.ToUniversalTime();

            return timeZone.ConvertTimeToTimeZone(date, TimeZoneInfo.Utc);
        }

        public static string DisplayDateTime(this DateTime date, string timeZoneId = null)
        {
            date = ConvertTimeToTimeZone(date, timeZoneId);
            return $"{date.ToShortDateString()} {date.ToShortTimeString()}";
        }

        public static string DisplayDate(this DateTime date, string timeZoneId = null)
        {
            date = ConvertTimeToTimeZone(date, timeZoneId);
            return date.ToShortDateString();
        }

        public static string DisplayDateTime(this DateTime? date, string timeZoneId = null)
        {
            if (date.HasValue)
                return DisplayDateTime(date.Value, timeZoneId);

            return "";
        }

        public static string DisplayDate(this DateTime? date, string timeZoneId = null)
        {
            if (date.HasValue)
                return DisplayDate(date.Value, timeZoneId);

            return "";
        }
    }
}