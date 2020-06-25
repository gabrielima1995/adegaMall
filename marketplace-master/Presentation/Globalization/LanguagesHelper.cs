using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LH = NewContext.Infrastructure.Languages.LanguagesHelper;

namespace Presentation.Globalization
{
    public static class LanguagesHelper
    {
        public static IReadOnlyList<LanguageInfo> ActiveLanguages
            => LH.ActiveLanguages.Select(l => LanguageInfo.FromSpecifier(l)).ToList().AsReadOnly();

        public static LanguageInfo DefaultLanguage
            => LanguageInfo.FromSpecifier(LH.DefaultLanguage);

        public static LanguageInfo GetLanguageInfo(string specifier)
            => LanguageInfo.FromSpecifier(specifier);

        public static bool IsLanguageActive(string specifier)
            => LH.IsLanguageActive(specifier);
    }

    public class LanguageInfo
    {
        private LanguageInfo(string specifier)
        {
            Specifier = specifier;
        }

        public string DisplayName => Culture.DisplayName;
        public string EnglishName => Culture.EnglishName;
        public string NativeName => Culture.NativeName;
        public string FlagImage => flagImages[Specifier];
        public string Specifier { get; }
        public CultureInfo Culture => CultureInfo.CreateSpecificCulture(Specifier);

        public static LanguageInfo PtBr => new LanguageInfo("pt-BR");
        public static LanguageInfo EnUs => new LanguageInfo("en-US");
        public static LanguageInfo CaEs => new LanguageInfo("ca-ES");
        public static LanguageInfo EsEs => new LanguageInfo("es-ES");
        public static LanguageInfo FromSpecifier(string specifier)
        {
            if (!LH.IsLanguageActive(specifier))
                throw new Exception($"Language '{specifier}' is not available.");

            return new LanguageInfo(specifier);
        }

        // Get flags at http://flag-icon-css.lip.is/
        private static readonly Dictionary<string, string> flagImages = new Dictionary<string, string>
        {
            {"ca-ES", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPoAAACnBAMAAADK0lwnAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAFVBMVEX83QnsghHaEhr4xwvxmQ/1sA3////mL+RMAAAAAWJLR0QGYWa4fQAAAAd0SU1FB+EICBIzOVDEm48AAAB2SURBVHja7dtRCcAwEETBsxALtVALtRD/VmriYGGZMfB+Fy6ZAQAocpLmSVJXV1dXV1dXX66/SYYtALDsS7Jp1dXV1dXV1avqN8mwBQCWZbeNTauurq6urq7eVM/e4wAAdmXfUdu06urq6urq6k317N9/AIAiP9ldg27iXq8aAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDE3LTA4LTA4VDE4OjUxOjU3KzAwOjAwscwmyAAAACV0RVh0ZGF0ZTptb2RpZnkAMjAxNy0wOC0wOFQxODo1MTo1NyswMDowMMCRnnQAAAAASUVORK5CYII=" },
            {"en-US", "https://lipis.github.io/flag-icon-css/flags/4x3/us.svg" },
            {"es-ES", "https://lipis.github.io/flag-icon-css/flags/4x3/es.svg" },
            {"pt-BR", "https://lipis.github.io/flag-icon-css/flags/4x3/br.svg" }
        };
    }
}