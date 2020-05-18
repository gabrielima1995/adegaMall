using Infra.Crosscutting.Globalization;

namespace System.Threading
{
    public static class GlobalizationThreadHelper
    {
        public static string GetCurrentLang(this Thread thread)
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public static string GetStringLocalized(this Thread thread, string label)
        {
            return GlobalizationHelper.GetString(label, thread.GetCurrentLang());
        }

        public static string FormatStringLocalized(this Thread thread, string label, params object[] args)
        {
            return GlobalizationHelper.FormatStringWithLanguage(label, thread.GetCurrentLang(), args);
        }
    }
}