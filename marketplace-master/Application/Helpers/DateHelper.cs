using System;

namespace Application.Helpers
{
    public static class DateHelper
    {
        public static DateTime FirstDayOfMonth
        {
            get
            {
                var today = DateTime.Today;
                return new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        public static DateTime LastDayOfMonth
        {
            get
            {
                return FirstDayOfMonth.AddMonths(1).AddSeconds(-1D);
            }
        }

        public static DateTime ToEndOfDay(DateTime date)
        {
            var zero = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            return zero.AddDays(1D).AddSeconds(-1D);
        }
    }
}
