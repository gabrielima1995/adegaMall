namespace System.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ActionAbsolute(this UrlHelper url, string actionName, string controllerName, object routeValues = null)
        {
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

            return url.Action(actionName, controllerName, routeValues, scheme);
        }
    }
}