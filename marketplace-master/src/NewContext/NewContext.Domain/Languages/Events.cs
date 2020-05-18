using NewContext.Core;

namespace NewContext.Domain.Languages
{
    public class LanguageActivated : IDomainEvent
    {
        public LanguageActivated(Language language)
        {
            Language = language;
        }

        public Language Language { get; }
    }

    public class LanguageDeactivated : IDomainEvent
    {
        public LanguageDeactivated(Language language)
        {
            Language = language;
        }

        public Language Language { get; }
    }

    public class LanguageSetAsDefault : IDomainEvent
    {
        public LanguageSetAsDefault(Language newDefault)
        {
            NewDefault = newDefault;
        }

        public Language NewDefault { get; }
    }

    public class LocalizedStringChanged : IDomainEvent
    {
        public LocalizedStringChanged(Language language, Label label, string newValue)
        {
            Language = language;
            Label = label;
            NewValue = newValue;
        }

        public Language Language { get; }
        public Label Label { get; }
        public string NewValue { get; }
    }

    public class LocalizedStringReset : IDomainEvent
    {
        public LocalizedStringReset(Language language, Label label, string defaultValue)
        {
            Language = language;
            Label = label;
            DefaultValue = defaultValue;
        }

        public Language Language { get; }
        public Label Label { get; }
        public string DefaultValue { get; }
    }
}
