using Domain.Interfaces;
using Microsoft.AspNet.Identity;

namespace System.Web.Mvc
{
    public class AuthorizeEnabledAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            var authProvider = DependencyResolver.Current.GetService<IUserAuthProvider>();
            var loginProfile = authProvider.GetLoginProfile();

            if (loginProfile == null || !loginProfile.IsEnabled)
            {
                httpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                httpContext.Session.RemoveAll();
                return false;
            }

            return true;
        }
    }
}