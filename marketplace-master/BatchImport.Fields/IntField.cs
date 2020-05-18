namespace BatchImport.Fields
{
    public class IntField : NumberField<int>
    {
        public IntField(string name, string displayName, string sample, bool isMandatory = false, int? minValue = null, int? maxValue = null)
            : base(name, displayName, sample, isMandatory, minValue, maxValue)
        {
            InvalidTypeErrorMessage = "Deve ser um número inteiro";
        }

        protected override int ClrParse(string input) => int.Parse(input);

        protected override bool ClrTryParse(string input, out int output) => int.TryParse(input, out output);
    }
}
