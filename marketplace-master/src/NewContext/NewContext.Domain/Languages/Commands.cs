using NewContext.Core;

namespace NewContext.Domain.Languages
{
    public class ActivateLanguage : ICommand
    {
        public ActivateLanguage(Language language)
        {
            Language = language;
        }

        public Language Language { get; }
    }

    public class ChangeLocalizedString : ICommand
    {
        public ChangeLocalizedString(Language language, Label label, string newValue)
        {
            Language = language;
            Label = label;
            NewValue = newValue;
        }

        public Language Language { get; }
        public Label Label { get; }
        public string NewValue { get; }
    }

    public class DeactivateLanguage : ICommand
    {
        public DeactivateLanguage(Language language)
        {
            Language = language;
        }

        public Language Language { get; }
    }

    public class ResetLocalizedString : ICommand
    {
        public ResetLocalizedString(Language language, Label label)
        {
            Language = language;
            Label = label;
        }

        public Language Language { get; }
        public Label Label { get; }
    }

    public class SetLanguageAsDefault : ICommand
    {
        public SetLanguageAsDefault(Language language)
        {
            Language = language;
        }

        public Language Language { get; }
    }
}
