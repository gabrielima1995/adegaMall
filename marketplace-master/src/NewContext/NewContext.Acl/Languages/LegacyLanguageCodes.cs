using NewContext.Domain.Languages;
using System.Collections.Generic;

namespace NewContext.Acl.Languages
{
    public static class LegacyLanguageCodes
    {
        private static readonly Dictionary<int, string> legacyToLanguage = new Dictionary<int, string>
        {
            { 1, "en-US" },
            { 2, "pt-BR" },
            { 3, "es-ES" },
            { 4, "ca-ES" }
        };

        private static readonly Dictionary<string, int> languageToLegacy = new Dictionary<string, int>
        {
            { "en-US", 1 },
            { "pt-BR", 2 },
            { "es-ES", 3 },
            { "ca-ES", 4 }
        };

        public static Language GetLanguageByLegacyCode(int legacyCode) => (Language)legacyToLanguage[legacyCode];

        public static int GetLecacyCode(Language language) => languageToLegacy[language];
    }
}
