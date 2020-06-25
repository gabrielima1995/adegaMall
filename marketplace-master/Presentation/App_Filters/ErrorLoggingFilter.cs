using Infrastructure;
using System.Web.Mvc;

namespace Presentation
{
    public class ErrorLoggingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            UnityConfig.Resolve<ErrorReportingService>().ReportError(filterContext.Exception);

            if (!(filterContext.HttpContext?.Request?.IsLocal ?? false))
                filterContext.HttpContext.Response.Redirect("~/error");
        }
    }
}