namespace System
{
    public static class DateTimeExtensions
    {
        public static int GetUnixTimestamp(this DateTime dateTime)
        {
            var refDate = new DateTime(1970, 1, 1);

            return dateTime <= refDate ? 0 :
                (int)(dateTime.Subtract(refDate)).TotalSeconds;
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(
                dateTime.Year, dateTime.Month, dateTime.Day,
                0, 0, 0, dateTime.Kind)
                    .AddDays(1).AddSeconds(-1D);
        }

        public static bool IsBusinessDay(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Sunday) return false;
            if (dateTime.DayOfWeek == DayOfWeek.Saturday) return false;
            return true;
        }

        public static DateTime GetBusinessDay(this DateTime dateTime)
        {
            while (!dateTime.IsBusinessDay()) dateTime = dateTime.AddDays(1D);
            return dateTime;
        }

        /// <summary>
        /// Arrendonda o horário para a primeira hora acima.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToHourUp(this DateTime dateTime)
        {
            // round seconds
            dateTime = dateTime.AddSeconds(60 - dateTime.Second);

            // round minutes
            dateTime = dateTime.AddMinutes(60 - dateTime.Minute);

            return dateTime;
        }
    }
}
