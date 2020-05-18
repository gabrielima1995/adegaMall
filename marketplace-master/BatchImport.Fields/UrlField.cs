using System;

namespace BatchImport.Fields
{
    public class UrlField : FieldBase
    {
        public UrlField(string name, string displayName, string sample, bool isMandatory = false)
            : base(name, displayName, sample, isMandatory)
        {
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            return new Uri(input).ToString();
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !Uri.TryCreate(input, UriKind.Absolute, out Uri test))
                validation.AddError("Deve ser uma URL válida (começando com http:// ou https://)");

            return validation;
        }
    }
}
