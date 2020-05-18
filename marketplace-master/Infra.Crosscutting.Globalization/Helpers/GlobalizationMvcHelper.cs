using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class GlobalizationMvcHelper
    {
        public static string ActionWithLang(this UrlHelper urlHelper, string actionName, string controllerName)
        {
            return ActionWithLang(urlHelper, actionName, controllerName, new RouteValueDictionary());
        }

        public static string ActionWithLang(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues)
        {
            return ActionWithLang(urlHelper, actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        public static string ActionWithLang(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            if (routeValues == null)
            {
                routeValues = new RouteValueDictionary();
            }

            routeValues.Add("lang", urlHelper.RequestContext.HttpContext.Request.GetCurrentLang());

            return urlHelper.Action(actionName, controllerName, routeValues);
        }

        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.Add(typeof(DateTime), new DateTimeBinder());
            binders.Add(typeof(DateTime?), new NullableDateTimeBinder());
            binders.Add(typeof(decimal), new DecimalBinder());
            binders.Add(typeof(decimal?), new NullableDecimalBinder());
        }
    }
}