using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class RequiredLocalizedAttribute : RequiredAttribute
    {
        public string StringLabel { get; }

        public RequiredLocalizedAttribute(string stringLabel = "REQUIRED_ERROR")
        {
            StringLabel = stringLabel;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name);
        }
    }
}