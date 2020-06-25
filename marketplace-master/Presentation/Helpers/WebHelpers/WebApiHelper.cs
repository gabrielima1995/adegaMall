using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace System.Web.Http
{
    public static class WebApiHelper
    {
        public static T GetService<T>(this IDependencyResolver dependencyResolver)
        {
            return (T)dependencyResolver.GetService(typeof(T));
        }

        public static HttpResponseMessage BadRequest(this HttpActionContext actionContext)
        {
            return actionContext.Request.CreateResponse(HttpStatusCode.BadRequest,
                new
                {
                    errors = actionContext.ModelState.Values.SelectMany(i => i.Errors).Select(i => i.ErrorMessage),
                    message = "Validation fail."
                });
        }
    }
}