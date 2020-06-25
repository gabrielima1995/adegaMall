using System.Collections.Generic;

namespace Application.Config.General
{
    public class LocalizationConfig
    {
        public string DefaultLang { get; set; } = "pt-br";

        public bool EnUsEnabled { get; set; } = false;

        public bool PtBrEnabled { get; set; } = true;

        public IList<string> GetSupportedLanguages()
        {
            return new List<string> { "pt-br", "en-us" };
        }

        public IList<string> GetEnabledLanguages()
        {
            var langs = GetSupportedLanguages();

            if (!PtBrEnabled) langs.Remove("pt-br");
            if (!EnUsEnabled) langs.Remove("en-us");

            return langs;
        }
    }
}