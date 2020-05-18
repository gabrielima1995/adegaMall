using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class RegularExpressionLocalizedAttribute : RegularExpressionAttribute
    {
        public string StringLabel { get; }

        public RegularExpressionLocalizedAttribute(string pattern, string stringLabel = "REGEX_ERROR")
            : base(pattern)
        {
            StringLabel = stringLabel;
        }

        public override string FormatErrorMessage(string name)
        {
            base.FormatErrorMessage(name);

            return string.Format(CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name, Pattern);
        }
    }
}