using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class CompareLocalizedAttribute : CompareAttribute
    {
        public string StringLabel { get; }

        public CompareLocalizedAttribute(string otherProperty, string stringLabel)
            : base(otherProperty)
        {
            StringLabel = stringLabel;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name);
        }
    }
}