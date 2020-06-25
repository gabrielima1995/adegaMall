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
                var personId = personRepository.GetPersonIdForUser(userId, PreferredPersonIdHelper.Get());

                authProvider.Set(userId, personId);
                PreferredPersonIdHelper.Set(personId);
            }

            return true;
        }
    }

    public static class PreferredPersonIdHelper
    {
        private const string key = "f74b7a6745cd7658e294";

        public static string Get()
        {
            var context = HttpContext.Current;
            return context.Request.Cookies[key]?.Value;
        }

        public static void Set(string personId)
        {
            var response = HttpContext.Current.Response;
            response.Cookies[key].Value = personId;
            response.Cookies[key].Expires = DateTime.Today.AddDays(30);
        }
    }
}