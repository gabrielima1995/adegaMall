using Domain.DTO;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class AuthorizeProfileAttribute : FilterAttribute, IAuthorizationFilter
    {
        public abstract ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes { get; }

        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            var profile = httpContext.User.Identity.GetLoginProfile();

            if (!profile.IsEnabled)
            {
                httpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                httpContext.Session.RemoveAll();
                return false;
            }

            if (profile.CurrentPersonType == LoginProfilePersonType.Unset)
                return false;

            if (!LoginProfileTypes.Contains(profile.CurrentPersonType))
                return false;

            return true;
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                // If a child action cache block is active, we need to fail immediately, even if authorization
                // would have succeeded. The reason is that there's no way to hook a callback to rerun
                // authorization before the fragment is served from the cache, so we can't guarantee that this
                // filter will be re-run on subsequent requests.
                throw new InvalidOperationException("AuthorizeAttribute_CannotUseWithinChildActionCache");
            }

            var skipAuthorization =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
                return;

            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else HandleUnauthorizedRequest(filterContext);
        }

        protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            bool isAuthorized = AuthorizeCore(httpContext);
            return isAuthorized ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        internal static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
                return Array.Empty<string>();

            var split =
                from piece in original.Split(',')
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;

            return split.ToArray();
        }

        protected ReadOnlyCollection<LoginProfilePersonType> Build(params LoginProfilePersonType[] types) => new List<LoginProfilePersonType>(types).AsReadOnly();
    }

    public sealed class AuthorizeAdminAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Admin, LoginProfilePersonType.MasterAdmin);
    }

    public sealed class AuthorizeBuyerAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Buyer);
    }

    public sealed class AuthorizeBuyerOrAdminAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Buyer, LoginProfilePersonType.Admin, LoginProfilePersonType.MasterAdmin);
    }

    public sealed class AuthorizeBuyerOrSellerAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Buyer, LoginProfilePersonType.Seller);
    }

    public sealed class AuthorizeBuyerOrSellerOrSellerAssistantAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Buyer, LoginProfilePersonType.Seller, LoginProfilePersonType.SellerAssistant);
    }

    public sealed class AuthorizeMasterAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.MasterAdmin);

    }

    public sealed class AuthorizeSellerAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Seller);
    }

    public sealed class AuthorizeSellerOrSellerAssistantAttribute : AuthorizeProfileAttribute
    {
        public override ReadOnlyCollection<LoginProfilePersonType> LoginProfileTypes => Build(LoginProfilePersonType.Seller, LoginProfilePersonType.SellerAssistant);
    }
}