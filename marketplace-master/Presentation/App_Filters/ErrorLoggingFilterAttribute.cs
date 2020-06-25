using Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Presentation
{
    public class ErrorLoggingFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            UnityConfig.Resolve<ErrorReportingService>().ReportError(actionExecutedContext.Exception);

            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}