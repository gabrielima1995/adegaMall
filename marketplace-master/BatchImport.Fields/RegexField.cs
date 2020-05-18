using System.Text.RegularExpressions;

namespace BatchImport.Fields
{
    public abstract class RegexField : FieldBase
    {
        public string ErrorMessage { get; protected set; }

        public Regex Regex { get; protected set; }

        public RegexField(string name, string displayName, string sample, string pattern, bool isMandatory = false, string errorMessage = null)
            : base(name, displayName, sample, isMandatory)
        {
            Regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            ErrorMessage = errorMessage;
        }

        public override object Parse(string input) => input;

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !Regex.IsMatch(input))
                validation.AddError(ErrorMessage ?? "Não está num formato válido");

            return validation;
        }
    }

}
