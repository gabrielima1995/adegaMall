namespace Infra.BatchImportAdapter.Fields
{
    public class CnpjField : FormattedRegexField
    {
        private const string pattern = @"^([0-9]{2})[\.]?([0-9]{3})[\.]?([0-9]{3})[\/]?([0-9]{4})[-]?([0-9]{2})$";
        private const string format = @"$1.$2.$3/$4-$5";

        public CnpjField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um CNPJ")
            : base(name, displayName, sample, pattern, format, isMandatory, errorMessage)
        {
        }
    }
}
