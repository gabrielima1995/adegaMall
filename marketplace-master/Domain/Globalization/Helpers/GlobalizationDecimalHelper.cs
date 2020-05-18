namespace System.Globalization
{
    public static class GlobalizationDecimalHelper
    {
        public static string ToStringLocalized(this decimal o, string lang, string format = "N2")
        {
            return o.ToString(format, CultureInfo.GetCultureInfo(lang));
        }
    }
}