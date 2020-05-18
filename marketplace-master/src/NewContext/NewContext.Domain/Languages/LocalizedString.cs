using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace NewContext.Domain.Languages
{
    public class LocalizedString : ValueObject
    {
        private LocalizedString(Language language, Label label, string value)
        {
            Language = language;
            Label = label;
            Value = value;
        }

        public Language Language { get; private set; }
        public Label Label { get; private set; }
        public string Value { get; private set; }

        public static Result<LocalizedString> Create(Language language, Label label, string value)
        {
            return Result.Ok(new LocalizedString(language, label, value));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Language;
            yield return Value;
        }

        public override string ToString() => $"{Language}\t{Label}\t{Value}";
    }
}
