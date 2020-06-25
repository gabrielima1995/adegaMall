using Domain.Entities;
using Infrastructure.Data.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using Unity;
using Unity.Injection;

namespace Presentation.Configuration
{
    public static class IdentityConfigurationExtensions
    {
        public static IUnityContainer AddIdentity(this IUnityContainer container)
        {
            container
                .RegisterType<UserManager<ApplicationUser>, ApplicationUserManager>(new ScopedLifetimeManager())
                .RegisterType<SignInManager<ApplicationUser, string>, ApplicationSignInManager>(new ScopedLifetimeManager())
                .RegisterType<IUserStore<ApplicationUser>>(new ScopedLifetimeManager(), new InjectionFactory(_ => new UserStore<ApplicationUser>(_.Resolve<ScaffoldContext>())))
                .RegisterType<IAuthenticationManager>(new ScopedLifetimeManager(), new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));

            return container;
        }
    }
}