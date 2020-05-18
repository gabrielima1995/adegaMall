using BatchImport.Fields;

namespace Infra.BatchImportAdapter.Fields
{
    public class BrazilianZipCodeField : RegexField
    {
        private const string pattern = @"^(\d{2}[\.\-\s]?\d{3})[\.\-\s]?(\d{3})$";

        public BrazilianZipCodeField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um CEP (formato 99999-99)")
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
        }
    }
}
