using System.Web.Mvc;

namespace Presentation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CookieConsentAttribute());
            filters.Add(new SetCurrentCultureAttribute());
            filters.Add(new SetUserLoggedAttribute());

            RegisterUtmFilters(filters);
        }

        private static void RegisterUtmFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CaptureUtmFilter());
            filters.Add(new RegisterUtmConversionFilter());
        }
    }
}