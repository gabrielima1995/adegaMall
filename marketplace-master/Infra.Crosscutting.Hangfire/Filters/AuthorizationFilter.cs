namespace Hangfire.Dashboard
{
    /// <summary>
    /// Filter responsible for authorizing access to the Hangfire dashboard.
    /// </summary>
    public class AuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            //var owin = new OwinContext(context.GetOwinEnvironment());

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            //return owin.Authentication.User.Identity.IsAuthenticated;

            return true;
        }
    }
}