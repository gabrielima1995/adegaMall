using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class RangeLocalizedAttribute : RangeAttribute
    {
        public string StringLabel { get; }

        public RangeLocalizedAttribute(int minimum, int maximum, string stringLabel = "RANGE_ERROR")
            : base(minimum, maximum)
        {
            StringLabel = stringLabel;
        }

        public RangeLocalizedAttribute(double minimum, double maximum, string stringLabel = "RANGE_ERROR") : base(minimum, maximum)
        {
            StringLabel = stringLabel;
        }

        public RangeLocalizedAttribute(Type type, string minimum, string maximum, string stringLabel = "RANGE_ERROR") : base(type, minimum, maximum)
        {
            StringLabel = stringLabel;
        }

        public override string FormatErrorMessage(string name)
        {
            base.FormatErrorMessage(name);

            return string.Format(CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name, Minimum, Maximum);
        }
    }
}