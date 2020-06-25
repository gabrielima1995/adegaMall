using Application.Config;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation
{
    public class CookieConsentAttribute : ActionFilterAttribute
    {
        public const string ConsentCookieName = "CookieConsent";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewBag = filterContext.Controller.ViewBag;
            viewBag.AskCookieConsent = true;
            viewBag.HasCookieConsent = false;

            var request = filterContext.HttpContext.Request;
            var consentCookie = request.Cookies[ConsentCookieName];

            if (consentCookie == null)
            {
                var dnt = request.Headers.Get("DNT" /* Do Not Track header, can be 0 or 1 */);
                var hasDntHeader = !string.IsNullOrEmpty(dnt);

                if (hasDntHeader)
                {
                    viewBag.AskCookieConsent = false;
                    if (dnt == "0") viewBag.HasCookieConsent = true;
                }
                else
                {
                    if (IsSearchCrawler(request.Headers.Get("User-Agent")))
                        viewBag.AskCookieConsent = false;

                    else
                    {
                        consentCookie = new HttpCookie(ConsentCookieName) { Value = "asked" };
                        filterContext.HttpContext.Response.Cookies.Add(consentCookie);
                    }
                }
            }
            else
            {
                if (consentCookie.Value == "asked")
                    return;

                viewBag.AskCookieConsent = false;
                viewBag.HasCookieConsent = consentCookie.Value == "true";
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsSearchCrawler(string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                var crawlers = new string[]
                {
                    "Baiduspider",
                    "Googlebot",
                    "YandexBot",
                    "YandexImages",
                    "bingbot",
                    "msnbot",
                    "Vagabondo",
                    "SeznamBot",
                    "ia_archiver",
                    "AcoonBot",
                    "Yahoo! Slurp",
                    "AhrefsBot"
                };

                if (crawlers.Any(c => userAgent.Contains(c)))
                    return true;
            }

            return false;
        }
    }

    public static class CookieConsent
    {
        public static void SetCookieConsent(HttpResponseBase response, bool consent)
        {
            var consentCookie = new HttpCookie(CookieConsentAttribute.ConsentCookieName)
            {
                Value = consent ? "true" : "false",
                Expires = DateTime.UtcNow.AddYears(1)
            };

            response.Cookies.Set(consentCookie);
        }

        public static bool AskCookieConsent() => MaaSConfig.Current.Gdpr.ShowCookieConsentMessage;
    }
}