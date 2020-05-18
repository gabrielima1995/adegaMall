using Infra.Crosscutting.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class PhoneNumberLocalized : RegularExpressionAttribute
    {
        private const string Label = "INVALID_PHONE_NUMBER";

        public PhoneNumberLocalized() : base(@"^(\+\d{1,2}[\d\-\.\s]+)|(\(?0?\d{2}[\s\-\.\)]*9?[\s\-\.]*\d{4}[\s\-\.]*\d{4})$")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return GlobalizationHelper.GetString(Label);
        }
    }
}
