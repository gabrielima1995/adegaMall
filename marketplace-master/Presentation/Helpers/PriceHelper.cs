using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Presentation.Helpers
{
    public static class PriceHelper
    {
        public static string GetIntegerPart(this decimal? price)
        {
            if (price == null) return null;

            var culture = CultureInfo.CurrentUICulture;
            var priceStr = price.Value.ToString("N2", culture);

            var parts = priceStr.Split(
                new string[] { culture.NumberFormat.NumberDecimalSeparator },
                StringSplitOptions.None);

            return parts[0];
        }

        public static string GetCents(this decimal? price)
        {
            if (price == null) return null;

            var priceStr = price.Value.ToString("N2", CultureInfo.InvariantCulture);
            var regex = new Regex("(?<=[\\.])[0-9]+", RegexOptions.Compiled);

            if (regex.IsMatch(priceStr))
            {
                var decimalPlaces = regex.Match(priceStr).Value;
                return decimalPlaces;
            }

            return "00";
        }
    }
}