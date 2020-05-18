using BatchImport.Fields;
using Domain.Entities;
using Infra.BatchImportAdapter.Internals;

namespace Infra.BatchImportAdapter.Fields
{
    public class GenreField : RegexField
    {
        private const string Pattern = @"^[mMfF]$";

        public GenreField(string name, string displayName, string sample, bool isMandatory = false, string errorMessage = null)
            : base(name, displayName, sample, Pattern, isMandatory, errorMessage)
        {
            ErrorMessage = ErrorMessage ?? Localization.Get("MUST_BE_M_OR_F");
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            input = input.ToLower();

            switch (input)
            {
                case "m": return Genre.Male;
                case "f": return Genre.Female;
                default: return Genre.Other;
            }
        }
    }
}
