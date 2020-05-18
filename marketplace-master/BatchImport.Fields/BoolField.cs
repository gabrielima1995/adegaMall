namespace BatchImport.Fields
{
    public class BoolField : FieldBase
    {
        public BoolField(string name, string displayName, string sample, bool isMandatory = false)
            : base(name, displayName, sample, isMandatory) { }

        public override object Parse(string input)
        {
            if (input == null) return null;
            return bool.Parse(input);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !bool.TryParse(input, out bool test))
                validation.AddError($"Deve ser TRUE ou FALSE");

            return validation;
        }
    }
}
