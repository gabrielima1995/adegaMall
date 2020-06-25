using Application.Auth;
using Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Presentation
{
    public class AuthorizeApiTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static bool ShouldSkipAuthorization(HttpActionContext actionContext)
        {
            return
                actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any() ||
                actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (ShouldSkipAuthorization(actionContext))
                return continuation();

            try
            {
                var provider = UnityConfig.Resolve<ApiTokenAuthorizedProvider>();

                var status = provider.GetAuthStatus();

                switch (status)
                {
                    case AuthorizationStatus.OK:
                        return continuation();

                    case AuthorizationStatus.Unauthorized:
                        actionContext.Response = new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.Unauthorized,
                            RequestMessage = actionContext.ControllerContext.Request
                        };
                        break;

                    case AuthorizationStatus.Forbidden:
                        actionContext.Response = new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.Forbidden,
                            RequestMessage = actionContext.ControllerContext.Request
                        };
                        break;
                }
            }
            catch (Exception ex)
            {
                UnityConfig.Resolve<ErrorReportingService>().ReportError(ex);

                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    RequestMessage = actionContext.ControllerContext.Request
                };
            }

            // finaliza request retornando o response com o status code de erro
            return Task.FromResult(actionContext.Response);
        }
    }
}