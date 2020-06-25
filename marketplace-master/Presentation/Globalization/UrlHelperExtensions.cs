using Application.Config;
using Presentation.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class UrlHelperGlobalizationExtensions
    {
        public static string AddLang(this UrlHelper helper, string urlOrPath)
        {
            urlOrPath = urlOrPath.Equals("/") ? HttpContext.Current.Request.Url.ToString() : $"{helper.RequestContext.HttpContext.Request.Url.Scheme}://{helper.RequestContext.HttpContext.Request.Url.Authority}{urlOrPath}";

            var uri = new Uri(urlOrPath);

            var finalUrl = uri.ToString();
            if (finalUrl.Contains("?")) finalUrl = finalUrl.Substring(0, finalUrl.IndexOf("?"));

            var query = HttpUtility.ParseQueryString(uri.Query);
            var hasLang = query.AllKeys.Any(key => key.Equals("lang", StringComparison.OrdinalIgnoreCase));
            if (!hasLang) query.Add("lang", CultureInfo.CurrentUICulture.Name);

            var queryString = ConstructQueryString(query);
            finalUrl = finalUrl + "?" + queryString;

            return finalUrl;
        }

        public static string ActionLocalized(this UrlHelper urlHelper, string actionName, string controllerName, bool forceHttps = false)
        {
            return ActionLocalized(urlHelper, actionName, controllerName, new RouteValueDictionary(), forceHttps);
        }

        public static string ActionLocalized(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues, bool forceHttps = false)
        {
            return ActionLocalized(urlHelper, actionName, controllerName, new RouteValueDictionary(routeValues), forceHttps);
        }

        public static string ActionLocalized(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, bool forceHttps = false)
        {
            if (routeValues == null)
                routeValues = new RouteValueDictionary();

            routeValues.Add("lang", urlHelper.RequestContext.HttpContext.Request.GetCurrentLang());
            return urlHelper.Action(actionName, controllerName, routeValues, GetProtocol(forceHttps));
        }

        private static string GetProtocol(bool forceHttps)
        {
            var isSecure = forceHttps ||
                (HttpContext.Current != null && HttpContext.Current.Request.Url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) ||
                MaaSConfig.Current.Environment.AppUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase);
            return isSecure ? "https" : "http";
        }

        private static string ConstructQueryString(NameValueCollection parameters)
        {
            var items = new List<string>();

            foreach (string name in parameters)
                items.Add(string.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

            return string.Join("&", items.ToArray());
        }
    }
}