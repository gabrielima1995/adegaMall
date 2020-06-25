using Owin;
using V2.Application;

namespace Presentation.Configuration
{
    public static class ApplicationV2ConfigurationExtensions
    {
        public static IAppBuilder UseApplicationV2(this IAppBuilder app)
        {
            UnityConfig.Resolve<ApplicationStartup>()
                .Startup(() => app.UseHangfire());

            return app;
        }
    }
}