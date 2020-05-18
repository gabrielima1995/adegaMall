using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class StringLengthLocalizedAttribute : StringLengthAttribute
    {
        public string StringLabel { get; }

        public string StringWithMinLabel { get; }

        public StringLengthLocalizedAttribute(int maximumLength, string stringLabel = "STRING_LENGTH_ERROR", string stringWithMinLabel = "STRING_LENGTH_WITH_MIN_ERROR")
            : base(maximumLength)
        {
            StringLabel = stringLabel;
            StringWithMinLabel = stringWithMinLabel;
        }

        public override string FormatErrorMessage(string name)
        {
            base.FormatErrorMessage(name);

            return string.Format(
                ErrorMessage ?? CultureInfo.CurrentCulture.GetStringLocalized(
                    MinimumLength != 0 ? StringWithMinLabel : StringLabel),
                    name,
                    MaximumLength,
                    MinimumLength);
        }
    }
}