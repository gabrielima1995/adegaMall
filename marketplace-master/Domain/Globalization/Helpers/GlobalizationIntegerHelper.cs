namespace System.Globalization
{
    public static class GlobalizationIntegerHelper
    {
        public static string ToStringLocalized(this int o, string lang)
        {
            return o.ToString(CultureInfo.GetCultureInfo(lang));
        }
    }
}