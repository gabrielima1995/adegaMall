using BatchImport.Fields;

namespace Infra.BatchImportAdapter.Fields
{
    public class BrazilianPhoneNumberField : RegexField
    {
        private const string pattern = @"^(\+55)?\s?(\(?0?\d{2}\)?)\s?(9?\d{4})[\-\.\s]?(\d{4})$";

        public BrazilianPhoneNumberField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um telefone completo (formato XX 95555-5555)")
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
        }
    }
}
