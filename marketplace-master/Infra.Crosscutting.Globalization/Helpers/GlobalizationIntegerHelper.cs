namespace System.Globalization
{
    public static class GlobalizationIntegerHelper
    {
        public static string ToStringInLang(this int o, string lang)
        {
            return o.ToString(CultureInfo.GetCultureInfo(lang));
        }
    }
}