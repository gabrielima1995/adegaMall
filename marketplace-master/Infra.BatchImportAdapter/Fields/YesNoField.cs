using BatchImport.Fields;
using Infra.BatchImportAdapter.Internals;

namespace Infra.BatchImportAdapter.Fields
{
    public class YesNoField : RegexField
    {
        private const string pattern = @"^(y)|(yes)|(s)|(sim)|(n)|(no)|(n[aã]o)$";

        public YesNoField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = null)
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
            ErrorMessage = ErrorMessage ?? Localization.Get("MUST_BE_Y_OR_N");
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            input = input.ToLower();

            switch (input)
            {
                case "y":
                case "yes":
                case "s":
                case "sim": return true;

                case "n":
                case "no":
                case "nao":
                case "não": return false;
            }

            return null;
        }
    }
}
