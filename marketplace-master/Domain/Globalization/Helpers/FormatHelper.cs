using Application.Config;
using Framework;
using Infra.Crosscutting.Globalization;
using Infra.Crosscutting.Globalization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System
{
    [Obsolete("There's no replacement yet. Marking as obsolete to ease identification when removing formatting concerns from Domain.")]
    public static class FormatHelper
    {
        private static DateTime ConvertTimeToTimeZone(DateTime date, MarketplaceTimeZone timeZone)
        {
            if (timeZone == null)
                timeZone = TimeZones.Default;

            if (date.Kind == DateTimeKind.Local)
                date = date.ToUniversalTime();

            return timeZone.ConvertTimeToTimeZone(date, TimeZoneInfo.Utc);
        }

        public static string DisplayShortNumber(this int num, CultureInfo culture = null)
        {
            if (num >= 100000000)
                return (num / 1000000D).ToString("0.#M");

            if (num >= 1000000)
                return (num / 1000000D).ToString("0.#M");

            if (num >= 100000)
                return (num / 1000D).ToString("0.#k");

            if (num >= 10000)
                return (num / 1000D).ToString("0.#k");

            return num.ToString("#,0", culture ?? CultureInfo.CurrentCulture);
        }

        public static string DisplayDateTime(DateTime date, MarketplaceTimeZone timeZone, CultureInfo culture = null)
        {
            date = ConvertTimeToTimeZone(date, timeZone);

            using (new CultureContext(culture ?? CultureInfo.CurrentCulture))
            {
                var shortDate = date.ToShortDateString();
                var shortTime = date.ToShortTimeString();

                return $"{shortDate} {shortTime}";
            }
        }

        public static string DisplayDate(DateTime date, MarketplaceTimeZone timeZone, CultureInfo culture = null)
        {
            date = ConvertTimeToTimeZone(date, timeZone);

            using (new CultureContext(culture ?? CultureInfo.CurrentCulture))
            {
                return date.ToShortDateString();
            }
        }

        public static string DisplayTime(DateTime date, MarketplaceTimeZone timeZone, CultureInfo culture = null)
        {
            date = ConvertTimeToTimeZone(date, timeZone);

            using (new CultureContext(culture ?? CultureInfo.CurrentCulture))
            {
                return date.ToShortTimeString();
            }
        }

        public static string DisplayDateTime(DateTime? date, MarketplaceTimeZone timeZone, CultureInfo culture = null)
        {
            return date.HasValue 
                ? DisplayDateTime(date.Value, timeZone, culture) 
                : string.Empty;
        }

        public static string DisplayDate(DateTime? date, MarketplaceTimeZone timeZone, CultureInfo culture = null)
        {
            return date.HasValue 
                ? DisplayDate(date.Value, timeZone, culture) 
                : string.Empty;
        }

        public static string DisplayDecimal(this decimal value, int fixedPoint, CultureInfo culture = null)
        {
            var places = new string('0', fixedPoint);
            var format = $"#,0.{places}";
            return value.ToString(format, culture ?? CultureInfo.CurrentCulture);
        }

        public static string DisplayCurrency(this decimal value, CultureInfo culture = null)
        {
            return $"{MaaSConfig.Current.DefaultCurrency} {value.DisplayDecimal(2, culture)}";
        }

        public static string DisplayDescription(this Enum value)
        {
            var memberInfo = value
                .GetType()
                .GetMember(value.ToString())
                .First();

            var descriptionLocalizedAttr = memberInfo.GetCustomAttribute<DescriptionLocalizedAttribute>();
            if (descriptionLocalizedAttr != null)
                return GlobalizationHelper.GetString(descriptionLocalizedAttr.Description);

            var displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null)
                return GlobalizationHelper.GetString(displayAttr.Name);

            var descriptionAttr = memberInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
                return GlobalizationHelper.GetString(descriptionAttr.Description);

            return value.ToString();
        }

        public static string DisplayShortGuid(this Guid value)
        {
            return value.ToString().Substring(0, 8);
        }
    }
}