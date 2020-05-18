using BatchImport.Fields;

namespace Infra.BatchImportAdapter.Fields
{
    public class FormattedRegexField : RegexField
    {
        public string SubstitutionPattern { get; protected set; }

        public FormattedRegexField(string name, string displayName, string sample, string pattern, string substitutionPattern, bool isMandatory = false, string errorMessage = null)
            : base(name, displayName, sample, pattern, isMandatory, errorMessage)
        {
            SubstitutionPattern = substitutionPattern;
        }

        public virtual string ApplyFormatting(string input) => Regex.Replace(input, SubstitutionPattern);

        public override object Parse(string input) => ApplyFormatting(input);
    }
}
