namespace BatchImport.Fields
{
    public class DecimalField : NumberField<decimal>
    {
        public DecimalField(string name, string displayName, string sample, bool isMandatory = false, decimal? minValue = null, decimal? maxValue = null)
            : base(name, displayName, sample, isMandatory, minValue, maxValue)
        {
            InvalidTypeErrorMessage = "Deve ser um número decimal";
        }

        protected override decimal ClrParse(string input) => decimal.Parse(input);

        protected override bool ClrTryParse(string input, out decimal output) => decimal.TryParse(input, out output);
    }
}
