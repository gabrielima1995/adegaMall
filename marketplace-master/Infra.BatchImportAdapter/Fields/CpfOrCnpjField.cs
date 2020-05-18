using BatchImport;
using BatchImport.Fields;
using System;
using System.Text.RegularExpressions;

namespace Infra.BatchImportAdapter.Fields
{
    public class CpfOrCnpjField : FieldBase
    {
        private readonly Regex CnpjPattern = new Regex(@"^([0-9]{2})[\.]?([0-9]{3})[\.]?([0-9]{3})[\/]?([0-9]{4})[-]?([0-9]{2})$", RegexOptions.Compiled);
        private const string CnpjFormat = @"$1.$2.$3/$4-$5";

        private readonly Regex CpfPattern = new Regex(@"^([0-9]{3})[\.]?([0-9]{3})[\.]?([0-9]{3})[-]?([0-9]{2})$", RegexOptions.Compiled);
        private const string CpfFormat = @"$1.$2.$3-$4";

        public string ErrorMessage { get; protected set; }

        public CpfOrCnpjField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = "Deve ser um CPF ou CNPJ")
            : base(name, displayName, sample, isMandatory)
        {
            ErrorMessage = errorMessage;
        }

        public override object Parse(string input)
        {
            if (input == null) return null;

            if (CnpjPattern.IsMatch(input))
                return CnpjPattern.Replace(input, CnpjFormat);

            if (CpfPattern.IsMatch(input))
                return CpfPattern.Replace(input, CpfFormat);

            throw new FormatException(ErrorMessage);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !(CnpjPattern.IsMatch(input) || CpfPattern.IsMatch(input)))
                validation.AddError(ErrorMessage);

            return validation;
        }
    }
}
