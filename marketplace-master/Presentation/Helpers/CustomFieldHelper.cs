using Infra.Crosscutting.Globalization;

namespace Presentation.Helpers
{
    public class CustomFieldHelper
    {
        public static string LocalizeField(string value, string lang)
        {
            if (value?.ToLower() == "true")
                return GlobalizationHelper.GetString("AFFIRMATIVE_LABEL", lang);

            if (value?.ToLower() == "false")
                return GlobalizationHelper.GetString("NEGATIVE_LABEL", lang);

            return value;
        }

        public static bool IsLink(string value)
        {
            return value.StartsWith("http://") || value.StartsWith("https://");
        }
    }
}