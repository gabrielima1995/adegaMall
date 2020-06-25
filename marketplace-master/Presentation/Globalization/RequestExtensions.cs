using Infra.Crosscutting.Globalization;
using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace Presentation.Globalization
{
    public static class RequestExtensions
    {
        public static bool CheckCurrentLang(this HttpRequest request, string lang)
            => CheckCurrentLang(request.UserLanguages, request, lang);

        public static bool CheckCurrentLang(this HttpRequestBase request, string lang)
            => CheckCurrentLang(request.UserLanguages, request, lang);

        public static string GetCurrentLang(this HttpRequest request)
            => GetCurrentLang(request.UserLanguages, request);

        public static string GetCurrentLang(this HttpRequestBase request)
            => GetCurrentLang(request.UserLanguages, request);

        public static string GetLocalizedString(this HttpRequest request, string labelName)
            => GetLocalizedString(request, labelName, null);

        public static string GetLocalizedString(this HttpRequestBase request, string labelName)
            => GetLocalizedString(request, labelName, null);

        public static string GetLocalizedString(this HttpRequestBase request, Enum enumValue)
            => GlobalizationEnumHelper.GetDescriptionLocalized(enumValue);

        public static void SetCurrentCulture(this HttpRequest request)
            => SetCurrentCulture(request.UserLanguages, request);

        public static void SetCurrentCulture(this HttpRequestBase request)
            => SetCurrentCulture(request.UserLanguages, request);

        public static string GetLocalizedString(this HttpRequestBase request, string labelName, params object[] formatArgs)
        {
            var useFormat = formatArgs != null && formatArgs.Length > 0;

            return useFormat
                ? GlobalizationHelper.FormatStringWithLanguage(labelName, request.GetCurrentLang(), formatArgs)
                : GlobalizationHelper.GetString(labelName, request.GetCurrentLang());
        }

        public static string GetLocalizedString(this HttpRequest request, string labelName, params object[] formatArgs)
        {
            var useFormat = formatArgs != null && formatArgs.Length > 0;

            return useFormat
                ? GlobalizationHelper.FormatStringWithLanguage(labelName, request.GetCurrentLang(), formatArgs)
                : GlobalizationHelper.GetString(labelName, request.GetCurrentLang());
        }

        private static bool CheckCurrentLang(string[] userLanguages, HttpRequestProxy request, string lang)
        {
            return GetCurrentLang(userLanguages, request) == lang.ToLower();
        }

        private static string GetCurrentLang(string[] userLanguages, HttpRequestProxy request)
        {
            var language = request["lang"];

            if (!LanguagesHelper.IsLanguageActive(language))
                language = LanguagesHelper.DefaultLanguage.Specifier;

            return language;
        }

        private static void SetCurrentCulture(string[] userLanguages, HttpRequestProxy request)
        {
            var language = GetCurrentLang(userLanguages, request);

            Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture =
                new CultureInfo(language);
        }

        private class HttpRequestProxy
        {
            private readonly HttpRequest request;
            private readonly HttpRequestBase requestBase;

            public HttpRequestProxy(HttpRequest request) => this.request = request;
            public HttpRequestProxy(HttpRequestBase requestBase) => this.requestBase = requestBase;

            public string this[string key] => request?[key] ?? requestBase?[key];

            public static implicit operator HttpRequestProxy(HttpRequest request) => new HttpRequestProxy(request);
            public static implicit operator HttpRequestProxy(HttpRequestBase requestBase) => new HttpRequestProxy(requestBase);
        }
    }
}