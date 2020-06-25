using Application.Config;
using Infra.Crosscutting.Helpers;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class DocumentNumberLocalizedAttribute : ValidationAttribute
    {
        public string StringLabel { get; }
        public bool CanBeCpf { get; }
        public bool CanBeCnpj { get; }
        public bool ShouldCheck { get; }

        public DocumentNumberLocalizedAttribute(string stringLabel = "DOCUMENT_NUMBER_ERROR", bool forceCheck = false, bool canBeCpf = true, bool canBeCnpj = true)
        {
            StringLabel = stringLabel;
            CanBeCpf = canBeCpf;
            CanBeCnpj = canBeCnpj;
            ShouldCheck = forceCheck || !MaaSConfig.Current.Seller.DocumentNumber.AllowFreeText;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage ?? CultureInfo.CurrentCulture.GetStringLocalized(StringLabel), name);
        }

        public override bool IsValid(object value)
        {
            if (!ShouldCheck)
            {
                return !string.IsNullOrWhiteSpace(value?.ToString());
            }

            if (!string.IsNullOrWhiteSpace(value?.ToString()))
            {
                var valueStr = value.ToString();

                return (CanBeCnpj && CNPJHelper.IsValid(valueStr)) || (CanBeCpf && CPFHelper.IsValid(valueStr));
            }

            return true;
        }
    }
}