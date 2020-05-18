using CSharpFunctionalExtensions;
using NewContext.Core;
using System.Collections.Generic;

namespace NewContext.Domain.Languages
{
    public class PlatformLanguage : Entity
    {
#pragma warning disable IDE0044 // Readonly modifier not added because field is set by EF
        private List<Language> activeLanguages = new List<Language>();
#pragma warning restore IDE0044 // Add readonly modifier

        private PlatformLanguage(Language defaultLanguage)
        {
            activeLanguages.Add(defaultLanguage);
            DefaultLanguage = defaultLanguage;
        }

        public static Result<PlatformLanguage> Create(Language defaultLanguage)
        {
            var platformLanguage = new PlatformLanguage(defaultLanguage);
            return Result.Ok(platformLanguage);
        }

        public IReadOnlyCollection<Language> ActiveLanguages => activeLanguages.AsReadOnly();

        public Language DefaultLanguage { get; private set; }

        public Result ActivateLanguage(Language language)
        {
            if (activeLanguages.Contains(language))
                return Result.Fail($"Language '{language}' is already active.");

            activeLanguages.Add(language);
            AddDomainEvent(new LanguageActivated(language));
            return Result.Ok();
        }

        public Result ChangeDefaultLanguage(Language newDefault)
        {
            if (!activeLanguages.Contains(newDefault))
                return Result.Fail($"Cannot set language '{newDefault}' as default because it is not active.");

            if (DefaultLanguage == newDefault)
                return Result.Fail($"Language '{newDefault}' is already set as default.");

            DefaultLanguage = newDefault;
            AddDomainEvent(new LanguageSetAsDefault(newDefault));
            return Result.Ok();
        }

        public Result DeactivateLanguage(Language language)
        {
            if (!activeLanguages.Contains(language))
                return Result.Fail($"Language '{language}' is already inactive.");

            activeLanguages.Remove(language);
            AddDomainEvent(new LanguageDeactivated(language));
            return Result.Ok();
        }

        public override object GetId() => 0;
    }
}
