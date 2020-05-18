namespace System.Globalization
{
    public static class GlobalizationDecimalHelper
    {
        public static string ToStringInLang(this decimal o, string lang)
        {
            return o.ToString(CultureInfo.GetCultureInfo(lang));
        }
    }
}