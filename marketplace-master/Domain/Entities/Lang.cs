using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Infra.Crosscutting.Globalization
{
    /// <summary>
    /// Idioma de uma string.
    /// </summary>
    public enum Lang
    {
        /// <summary>
        /// Inglês (EUA).
        /// </summary>
        [Description("en-us")]
        en_US = 1,

        /// <summary>
        /// Português (Brasil).
        /// </summary>
        [Description("pt-br")]
        pt_BR = 2,

        /// <summary>
        /// Espanhol (Espanha).
        /// </summary>
        [Description("es-ES")]
        es_ES = 3,

        /// <summary>
        /// Catalão (Espanha).
        /// </summary>
        [Description("ca-ES")]
        ca_ES = 4
    }

    public static class LangHelper
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

        public static Lang GetLangFromCultureSpecifier(string cultureSpecifier)
        {
            if (!languageToLegacy.ContainsKey(cultureSpecifier))
                throw new Exception($"Culture or language '{cultureSpecifier}' is not supported.");

            return (Lang)languageToLegacy[cultureSpecifier];
        }

        public static string GetSpecifier(this Lang lang)
        {
            var code = (int)lang;

            if (!legacyToLanguage.ContainsKey(code))
                throw new Exception($"Culture or language '{lang.ToString()}' is not supported.");

            return legacyToLanguage[code];
        }
    }
}