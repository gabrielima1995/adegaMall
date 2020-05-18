using System.ComponentModel;
using System.Globalization;

namespace Infra.Crosscutting.Globalization.Attributes
{
    public class DescriptionLocalizedAttribute : DescriptionAttribute
    {
        public string StringLabel { get; }

        public DescriptionLocalizedAttribute(string stringLabel)
        {
            StringLabel = stringLabel;
        }

        public override string Description => CultureInfo.CurrentCulture.GetStringLocalized(StringLabel);
    }
}