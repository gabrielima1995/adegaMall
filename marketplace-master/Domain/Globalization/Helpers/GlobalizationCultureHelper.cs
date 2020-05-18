using Infra.Crosscutting.Globalization;

namespace System.Globalization
{
    public static class GlobalizationCultureHelper
    {
        public static string GetCurrentLang(this CultureInfo culture)
        {
            return culture.Name.ToLower();
        }

        public static string GetStringLocalized(this CultureInfo culture, string label)
        {
            return GlobalizationHelper.GetString(label, culture.GetCurrentLang());
        }
    }
}