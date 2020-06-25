namespace System.Web.Mvc
{
    public class SetCurrentCultureAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Request.SetCurrentCulture();

            base.OnActionExecuting(filterContext);
        }
    }
}