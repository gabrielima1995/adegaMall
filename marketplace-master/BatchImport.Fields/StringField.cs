namespace BatchImport.Fields
{
    public class StringField : FieldBase
    {
        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public StringField(string name, string displayName, string sample, bool isMandatory = false, int? maxLength = null, int? minLength = null)
            : base(name, displayName, sample, isMandatory)
        {
            MaxLength = maxLength;
            MinLength = minLength;
        }

        public override object Parse(string input) => input;

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && MaxLength != null && input.Length > MaxLength)
                validation.AddError($"Não pode ter mais de {MaxLength} caracteres");

            if (input != null && MinLength != null && input.Length < MinLength)
                validation.AddError($"Não pode ter mais de {MinLength} caracteres");

            return validation;
        }
    }
}
