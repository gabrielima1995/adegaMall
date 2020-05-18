using System;

namespace BatchImport.Fields
{
    public class DateField : FieldBase
    {
        public DateField(string name, string displayName, string sample, bool isMandatory = false)
            : base(name, displayName, sample, isMandatory)
        {
            base.IsMandatory = false;
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            return DateTime.Parse(input);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !DateTime.TryParse(input, out DateTime test))
                validation.AddError("Deve ser uma data válida");

            return validation;
        }
    }
}
