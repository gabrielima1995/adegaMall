using RazorEngine;
using RazorEngine.Templating;

namespace Application.Helpers
{
    public static class ResourcesHelper
    {
        private static string GetResourceName(string name, string lang)
        {
            var langFixed = lang.ToLower();

            return name.EndsWith(lang)
                ? name
                : name + "_" + langFixed;
        }

        public static string GetStringLocalized(string name, string lang)
        {
            return Domain.Resources.AppResources.ResourceManager.GetString(GetResourceName(name, lang));
        }

        public static string CompileRazorStringInLang(string name, string lang, object o)
        {
            var template = GetStringLocalized(name, lang);

            return Engine.Razor.RunCompile(template, GetResourceName(name, lang), null, o);
        }
    }
}