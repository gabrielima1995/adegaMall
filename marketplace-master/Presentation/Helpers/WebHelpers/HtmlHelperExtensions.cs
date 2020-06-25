namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static bool IsCurrent(this UrlHelper url, string controllerName)
        {
            var isController = url.RequestContext.RouteData.GetRequiredString("controller").Equals(controllerName, StringComparison.OrdinalIgnoreCase);
            return isController;
        }

        public static bool IsCurrent(this UrlHelper url, string actionName, string controllerName)
        {
            var isController = IsCurrent(url, controllerName);
            var isAction = url.RequestContext.RouteData.GetRequiredString("action").Equals(actionName, StringComparison.OrdinalIgnoreCase);
            return isController && isAction;
        }

        public static bool IsCurrent(this HtmlHelper html, string controllerName)
        {
            var isController = html.ViewContext.RouteData.GetRequiredString("controller").Equals(controllerName, StringComparison.OrdinalIgnoreCase);
            return isController;
        }

        public static bool IsCurrent(this HtmlHelper html, string actionName, string controllerName)
        {
            var isController = IsCurrent(html, controllerName);
            var isAction = html.ViewContext.RouteData.GetRequiredString("action").Equals(actionName, StringComparison.OrdinalIgnoreCase);
            return isController && isAction;
        }

        public static string ClassIfCurrent(this HtmlHelper html, string className, string controllerName)
        {
            return IsCurrent(html, controllerName) ? className : "";
        }

        public static string ClassIfCurrent(this HtmlHelper html, string className, string actionName, string controllerName)
        {
            return IsCurrent(html, actionName, controllerName) ? className : "";
        }

        public static string ClassIfCurrentController(this HtmlHelper html, string className, params string[] controllerNames)
        {
            foreach (var controllerName in controllerNames)
            {
                if (IsCurrent(html, controllerName)) return className;
            }

            return "";
        }

        public static MvcHtmlString HiddenFromQueryParam(this HtmlHelper html, string queryStringParameterName)
        {
            var value = html.ViewContext?.RequestContext?.HttpContext?.Request?.QueryString[queryStringParameterName];

            if (string.IsNullOrEmpty(value)) return new MvcHtmlString("");

            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes.Add("id", HtmlHelper.GenerateIdFromName(queryStringParameterName));
            tagBuilder.Attributes.Add("name", queryStringParameterName);
            tagBuilder.Attributes.Add("type", "hidden");
            tagBuilder.Attributes.Add("value", value);

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}