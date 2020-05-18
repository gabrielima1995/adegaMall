using System.Globalization;
using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    /// Specifies the display name for a property, event, or public void method
    /// which takes no arguments, with localization.
    /// </summary>
    public class DisplayNameLocalizedAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// The label of string in localization.
        /// </summary>
        public string StringLabel { get; }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.DisplayNameLocalizedAttribute
        /// class using the display name with localization.
        /// </summary>
        /// <param name="stringLabel">The label of string in localization.</param>
        public DisplayNameLocalizedAttribute(string stringLabel)
        {
            StringLabel = stringLabel;
        }

        public override string DisplayName => CultureInfo.CurrentCulture.GetStringLocalized(StringLabel);
    }
}