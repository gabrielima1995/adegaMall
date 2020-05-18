namespace Infra.BatchImportAdapter.Fields
{
    public class CpfField : FormattedRegexField
    {
        private const string pattern = @"^([0-9]{3})[\.]?([0-9]{3})[\.]?([0-9]{3})[-]?([0-9]{2})$";
        private const string format = @"$1.$2.$3-$4";

        public CpfField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um CPF")
            : base(name, displayName, sample, pattern, format, isMandatory, errorMessage)
        {
        }
    }
}
