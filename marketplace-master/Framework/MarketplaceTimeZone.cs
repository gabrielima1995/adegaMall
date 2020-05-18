using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class TimeZones
    {
        private static readonly List<MarketplaceTimeZone> zones;

        public static IReadOnlyCollection<MarketplaceTimeZone> All => zones.AsReadOnly();
        public static MarketplaceTimeZone Default => Find("E. South America Standard Time");
        public static MarketplaceTimeZone Find(string systemId) => zones.FirstOrDefault(x => x.TimeZoneId.Equals(systemId));

        static TimeZones()
        {
            zones = new List<MarketplaceTimeZone>()
            {
                new MarketplaceTimeZone("Dateline Standard Time", "(-12:00) International Date Line West"),
                new MarketplaceTimeZone("UTC-11", "(-11:00) Coordinated Universal Time -11"),
                new MarketplaceTimeZone("Samoa Standard Time", "(-11:00) Samoa"),
                new MarketplaceTimeZone("Hawaiian Standard Time", "(-10:00) Hawaii"),
                new MarketplaceTimeZone("Alaskan Standard Time", "(-09:00) Alaska"),
                new MarketplaceTimeZone("Pacific Standard Time", "(-08:00) Pacific Time (US and Canada)"),
                new MarketplaceTimeZone("Pacific Standard Time (Mexico)", "(-08:00) Baja California"),
                new MarketplaceTimeZone("Mountain Standard Time", "(-07:00) Mountain Time (US and Canada)"),
                new MarketplaceTimeZone("Mountain Standard Time (Mexico)", "(-07:00) Chihuahua, La Paz, Mazatlan"),
                new MarketplaceTimeZone("US Mountain Standard Time", "(-07:00) Arizona"),
                new MarketplaceTimeZone("Canada Central Standard Time", "(-06:00) Saskatchewan"),
                new MarketplaceTimeZone("Central America Standard Time", "(-06:00) Central America"),
                new MarketplaceTimeZone("Central Standard Time", "(-06:00) Central Time (US and Canada)"),
                new MarketplaceTimeZone("Central Standard Time (Mexico)", "(-06:00) Guadalajara, Mexico City, Monterrey"),
                new MarketplaceTimeZone("Eastern Standard Time", "(-05:00) Eastern Time (US and Canada)"),
                new MarketplaceTimeZone("SA Pacific Standard Time", "(-05:00) Bogota, Lima, Quito"),
                new MarketplaceTimeZone("US Eastern Standard Time", "(-05:00) Indiana (East)"),
                new MarketplaceTimeZone("Venezuela Standard Time", "(-04:30) Caracas"),
                new MarketplaceTimeZone("Atlantic Standard Time", "(-04:00) Atlantic Time (Canada)"),
                new MarketplaceTimeZone("Central Brazilian Standard Time", "(-04:00) Cuiaba"),
                new MarketplaceTimeZone("Pacific SA Standard Time", "(-04:00) Santiago"),
                new MarketplaceTimeZone("SA Western Standard Time", "(-04:00) Georgetown, La Paz, Manaus, San Juan"),
                new MarketplaceTimeZone("Paraguay Standard Time", "(-04:00) Asuncion"),
                new MarketplaceTimeZone("Newfoundland Standard Time", "(-03:30) Newfoundland"),
                new MarketplaceTimeZone("E. South America Standard Time", "(-03:00) Brasilia"),
                new MarketplaceTimeZone("Greenland Standard Time", "(-03:00) Greenland"),
                new MarketplaceTimeZone("Montevideo Standard Time", "(-03:00) Montevideo"),
                new MarketplaceTimeZone("SA Eastern Standard Time", "(-03:00) Cayenne, Fortaleza"),
                new MarketplaceTimeZone("Argentina Standard Time", "(-03:00) Buenos Aires"),
                new MarketplaceTimeZone("Mid-Atlantic Standard Time", "(-02:00) Mid-Atlantic"),
                new MarketplaceTimeZone("UTC-2", "(-02:00) Coordinated Universal Time -02"),
                new MarketplaceTimeZone("Azores Standard Time", "(-01:00) Azores"),
                new MarketplaceTimeZone("Cabo Verde Standard Time", "(-01:00) Cabo Verde Is."),
                new MarketplaceTimeZone("GMT Standard Time", "(GMT) Dublin, Edinburgh, Lisbon, London"),
                new MarketplaceTimeZone("Greenwich Standard Time", "(GMT) Monrovia, Reykjavik"),
                new MarketplaceTimeZone("Morocco Standard Time", "(GMT) Casablanca"),
                new MarketplaceTimeZone("Central Europe Standard Time", "(+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague"),
                new MarketplaceTimeZone("Central European Standard Time", "(+01:00) Sarajevo, Skopje, Warsaw, Zagreb"),
                new MarketplaceTimeZone("Romance Standard Time", "(+01:00) Brussels, Copenhagen, Madrid, Paris"),
                new MarketplaceTimeZone("W. Central Africa Standard Time", "(+01:00) West Central Africa"),
                new MarketplaceTimeZone("W. Europe Standard Time", "(+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"),
                new MarketplaceTimeZone("Namibia Standard Time", "(+01:00) Windhoek"),
                new MarketplaceTimeZone("E. Europe Standard Time", "(+02:00) Minsk"),
                new MarketplaceTimeZone("Egypt Standard Time", "(+02:00) Cairo"),
                new MarketplaceTimeZone("FLE Standard Time", "(+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius"),
                new MarketplaceTimeZone("GTB Standard Time", "(+02:00) Athens, Bucharest"),
                new MarketplaceTimeZone("Israel Standard Time", "(+02:00) Jerusalem"),
                new MarketplaceTimeZone("Jordan Standard Time", "(+02:00) Amman"),
                new MarketplaceTimeZone("Middle East Standard Time", "(+02:00) Beirut"),
                new MarketplaceTimeZone("South Africa Standard Time", "(+02:00) Harare, Pretoria"),
                new MarketplaceTimeZone("Syria Standard Time", "(+02:00) Damascus"),
                new MarketplaceTimeZone("Turkey Standard Time", "(+02:00) Istanbul"),
                new MarketplaceTimeZone("Arab Standard Time", "(+03:00) Kuwait, Riyadh"),
                new MarketplaceTimeZone("Arabic Standard Time", "(+03:00) Baghdad"),
                new MarketplaceTimeZone("E. Africa Standard Time", "(+03:00) Nairobi"),
                new MarketplaceTimeZone("Kaliningrad Standard Time", "(+03:00) Kaliningrad"),
                new MarketplaceTimeZone("Iran Standard Time", "(+03:30) Tehran"),
                new MarketplaceTimeZone("Russian Standard Time", "(+04:00) Moscow, St. Petersburg, Volgograd"),
                new MarketplaceTimeZone("Arabian Standard Time", "(+04:00) Abu Dhabi, Muscat"),
                new MarketplaceTimeZone("Azerbaijan Standard Time", "(+04:00) Baku"),
                new MarketplaceTimeZone("Caucasus Standard Time", "(+04:00) Yerevan"),
                new MarketplaceTimeZone("Georgian Standard Time", "(+04:00) Tbilisi"),
                new MarketplaceTimeZone("Mauritius Standard Time", "(+04:00) Port Louis"),
                new MarketplaceTimeZone("Afghanistan Standard Time", "(+04:30) Kabul"),
                new MarketplaceTimeZone("West Asia Standard Time", "(+05:00) Tashkent"),
                new MarketplaceTimeZone("Pakistan Standard Time", "(+05:00) Islamabad, Karachi"),
                new MarketplaceTimeZone("India Standard Time", "(+05:30) Chennai, Kolkata, Mumbai, New Delhi"),
                new MarketplaceTimeZone("Sri Lanka Standard Time", "(+05:30) Sri Jayawardenepura"),
                new MarketplaceTimeZone("Nepal Standard Time", "(+05:45) Kathmandu"),
                new MarketplaceTimeZone("Ekaterinburg Standard Time", "(+06:00) Ekaterinburg"),
                new MarketplaceTimeZone("Central Asia Standard Time", "(+06:00) Astana"),
                new MarketplaceTimeZone("Bangladesh Standard Time", "(+06:00) Dhaka"),
                new MarketplaceTimeZone("Myanmar Standard Time", "(+06:30) Yangon (Rangoon)"),
                new MarketplaceTimeZone("N. Central Asia Standard Time", "(+07:00) Novosibirsk"),
                new MarketplaceTimeZone("SE Asia Standard Time", "(+07:00) Bangkok, Hanoi, Jakarta"),
                new MarketplaceTimeZone("North Asia Standard Time", "(+08:00) Krasnoyarsk"),
                new MarketplaceTimeZone("China Standard Time", "(+08:00) Beijing, Chongqing, Hong Kong, Urumqi"),
                new MarketplaceTimeZone("Singapore Standard Time", "(+08:00) Kuala Lumpur, Singapore"),
                new MarketplaceTimeZone("Taipei Standard Time", "(+08:00) Taipei"),
                new MarketplaceTimeZone("W. Australia Standard Time", "(+08:00) Perth"),
                new MarketplaceTimeZone("Ulaanbaatar Standard Time", "(+08:00) Ulaanbaatar"),
                new MarketplaceTimeZone("North Asia East Standard Time", "(+09:00) Irkutsk"),
                new MarketplaceTimeZone("Korea Standard Time", "(+09:00) Seoul"),
                new MarketplaceTimeZone("Tokyo Standard Time", "(+09:00) Osaka, Sapporo, Tokyo"),
                new MarketplaceTimeZone("AUS Central Standard Time", "(+09:30) Darwin"),
                new MarketplaceTimeZone("Cen. Australia Standard Time", "(+09:30) Adelaide"),
                new MarketplaceTimeZone("Yakutsk Standard Time", "(+10:00) Yakutsk"),
                new MarketplaceTimeZone("AUS Eastern Standard Time", "(+10:00) Canberra, Melbourne, Sydney"),
                new MarketplaceTimeZone("E. Australia Standard Time", "(+10:00) Brisbane"),
                new MarketplaceTimeZone("Tasmania Standard Time", "(+10:00) Hobart"),
                new MarketplaceTimeZone("West Pacific Standard Time", "(+10:00) Guam, Port Moresby"),
                new MarketplaceTimeZone("Vladivostok Standard Time", "(+11:00) Vladivostok"),
                new MarketplaceTimeZone("Central Pacific Standard Time", "(+11:00) Solomon Is., New Caledonia"),
                new MarketplaceTimeZone("Magadan Standard Time", "(+12:00) Magadan"),
                new MarketplaceTimeZone("Fiji Standard Time", "(+12:00) Fiji"),
                new MarketplaceTimeZone("New Zealand Standard Time", "(+12:00) Auckland, Wellington"),
                new MarketplaceTimeZone("UTC+12", "(+12:00) Coordinated Universal Time +12"),
                new MarketplaceTimeZone("Tonga Standard Time", "(+13:00) Nuku'alofa")
            };
        }
    }

    public class MarketplaceTimeZone
    {
        private readonly Lazy<TimeZoneInfo> timeZoneInfo;

        public string TimeZoneId { get; }
        public string Description { get; }

        public TimeZoneInfo TimeZoneInfo => timeZoneInfo.Value;

        public MarketplaceTimeZone(string timeZoneId, string description)
        {
            TimeZoneId = timeZoneId;
            Description = description;
            timeZoneInfo = new Lazy<TimeZoneInfo>(() => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
        }

        public DateTimeWithZone DateTimeWithZone(DateTime dateTime)
        {
            return new DateTimeWithZone(dateTime, TimeZoneInfo);
        }

        public DateTimeWithZone CreateDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            return DateTimeWithZone(new DateTime(year, month, day, hour, minute, second));
        }

        public DateTime ConvertTimeToTimeZone(DateTime dateTime, TimeZoneInfo sourceTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZoneInfo);
        }

        public DateTime ConvertTimeToUtc(DateTime dateTime)
        {
            try
            {
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo);
            }
            catch (Exception)
            {
                return dateTime.Add(TimeZoneInfo.BaseUtcOffset);
            }
        }

        public DateTime? ConvertTimeToUtc(DateTime? dateTime)
        {
            if (dateTime == null) return null;
            return ConvertTimeToUtc(dateTime.Value);
        }

        public DateTime ConvertTimeFromUtc(DateTime dateTimeUTC)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUTC, TimeZoneInfo);
        }

        public override string ToString() => $"{Description} ({TimeZoneId})";
    }

    public struct DateTimeWithZone
    {
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            TimeZone = timeZone;
        }

        public TimeZoneInfo TimeZone { get; }
        public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
        public DateTime UniversalTime { get; }
    }
}