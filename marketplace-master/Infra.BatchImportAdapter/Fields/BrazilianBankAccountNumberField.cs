using BatchImport.Fields;

namespace Infra.BatchImportAdapter.Fields
{
    public class BrazilianBankAccountNumberField : RegexField
    {
        private const string pattern = @"^[\d\-]+$";

        public BrazilianBankAccountNumberField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve conter apenas números")
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
        }
    }
}
