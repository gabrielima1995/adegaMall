using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewContext.Domain.Languages
{
    public class Language : ValueObject
    {
        private static readonly string[] supportedLanguages = new string[] { "ca-ES", "en-US", "es-ES", "pt-BR" };

        private Language()
        {
        }

        private Language(string specifier)
        {
            Specifier = specifier;
        }

        public string Specifier { get; private set; }

        public static Result<Language> Create(string specifier)
        {
            var match = supportedLanguages.SingleOrDefault(lang
                => lang.Equals(specifier, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                return Result.Fail<Language>($"Language '{specifier}' is not supported.");

            return Result.Ok(new Language(match));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Specifier;
        }

        public static IEnumerable<Language> SupportedLanguages => supportedLanguages.Select(l => new Language(l));

        public override string ToString() => Specifier;

        public static implicit operator string(Language languageId) => languageId.Specifier;

        public static explicit operator Language(string id) => Create(id).Value;
    }
}