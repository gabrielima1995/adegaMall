using Framework;
using System;

namespace Domain.Interfaces
{
    public interface ITimeZoneProvider
    {
        MarketplaceTimeZone GetTimeZone();
    }

    public static class TimeZoneProverExtensions
    {
        public static DateTime FromUtc(this ITimeZoneProvider provider, DateTime dateTime)
        {
            return provider.GetTimeZone().ConvertTimeFromUtc(dateTime);
        }

        public static DateTime ToTimeZone(this ITimeZoneProvider provider, DateTime dateTime, TimeZoneInfo fromTimeZone)
        {
            return provider.GetTimeZone().ConvertTimeToTimeZone(dateTime, fromTimeZone);
        }

        public static DateTime ToUtc(this ITimeZoneProvider provider, DateTime dateTime)
        {
            return provider.GetTimeZone().ConvertTimeToUtc(dateTime);
        }

        public static DateTime? ToUtc(this ITimeZoneProvider provider, DateTime? dateTime)
        {
            return provider.GetTimeZone().ConvertTimeToUtc(dateTime);
        }
    }
}