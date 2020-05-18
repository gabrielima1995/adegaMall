using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Microsoft.AspNet.Identity;

namespace System.Web.Mvc
{
    public class SetUserLoggedAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                var authProvider = DependencyResolver.Current.GetService<IUserAuthProvider>();
                var personRepository = DependencyResolver.Current.GetService<IPersonRepository>();

                var userId = httpContext.User.Identity.GetUserId();
                var personId = personRepository.GetPersonIdForUser(userId);

                authProvider.Set(userId, personId);
            }

            return true;
        }
    }
}