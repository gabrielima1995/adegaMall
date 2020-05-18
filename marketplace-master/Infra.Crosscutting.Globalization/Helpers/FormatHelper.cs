using Infra.Crosscutting.Globalization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class FormatHelper
    {
        public static string DisplayShortNumber(this int num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }

            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }

            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }

            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.#k");
            }

            return num.ToString("#,0");
        }

        public static string DisplayDateTime(this DateTime date, string lang)
        {
            return date.ToString(CultureInfo.GetCultureInfo(lang));
        }

        public static string DisplayDateTime(this DateTime date)
        {
            return date.ToString(CultureInfo.CurrentUICulture);
        }

        public static string DisplayDate(this DateTime date)
        {
            return date.ToShortDateString();
        }

        public static string DisplayDecimal(this decimal value, int fixedPoint)
        {
            return value.ToString("F" + fixedPoint);
        }

        public static string DisplayDescription(this Enum value)
        {
            var memberInfo = value
                .GetType()
                .GetMember(value.ToString())
                .First();

            var displayAttr = memberInfo
                .GetCustomAttribute<DisplayAttribute>();

            var descriptionLocalizedAttr = memberInfo
                .GetCustomAttribute<DescriptionLocalizedAttribute>();

            if (descriptionLocalizedAttr != null)
            {
                return descriptionLocalizedAttr.Description;
            }

            if (displayAttr != null)
            {
                return displayAttr.Name;
            }

            var descriptionAttr = memberInfo
                .GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttr != null)
            {
                return descriptionAttr.Description;
            }

            return value.ToString();
        }

        public static string DisplayShortGuid(this Guid value)
        {
            return value.ToString().Substring(0, 8);
        }
    }
}