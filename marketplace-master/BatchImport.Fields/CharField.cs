namespace BatchImport.Fields
{
    public class CharField : FieldBase
    {
        public CharField(string name, string displayName, string sample, bool isMandatory = false)
            : base(name, displayName, sample, isMandatory)
        {
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            return char.Parse(input);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !char.TryParse(input, out char test))
                validation.AddError("Deve ser um único caracter");

            return validation;
        }
    }
}
