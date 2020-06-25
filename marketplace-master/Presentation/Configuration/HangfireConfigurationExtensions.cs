using Hangfire;
using Infra.Crosscutting.Hangfire;
using Owin;

namespace Presentation.Configuration
{
    public static class HangfireConfigurationExtensions
    {
        public static IAppBuilder UseHangfire(this IAppBuilder app)
        {
            var settings = new HangfireSettings();

            app.UseHangfireServer(new FuncJobActivator(UnityConfig.Resolve), settings);
            app.UseHangfirePanel(settings);

            return app;
        }
    }
}