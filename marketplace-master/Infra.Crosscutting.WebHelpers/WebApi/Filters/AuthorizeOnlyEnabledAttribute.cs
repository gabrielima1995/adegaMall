using Domain.Entities;
using Framework;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace System.Web.Http
{
    public class AuthorizeOnlyEnabledAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (base.IsAuthorized(actionContext))
            {
                var userRepository = GlobalConfiguration.Configuration.DependencyResolver.GetService<IRepository<ApplicationUser>>();

                var user = actionContext.RequestContext.Principal;
                var userId = user.Identity.GetUserId();

                if (userRepository.Query().Any(i => i.Id == userId && i.Enabled))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}