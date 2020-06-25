using NewContext.Application.Startup;
using Owin;
using System;
using System.Threading.Tasks;
using Unity;

namespace NewContext.Application
{
    public static class ApplicationConfigurationExtensions
    {
        public static IUnityContainer AddNewContextApplication(this IUnityContainer container)
        {
            container.RegisterType<IStartup, Languages.Startup>(typeof(Languages.Startup).FullName);

            HandlerRegistration.AddCommandHandlers(container);

            return container;
        }

        public static IAppBuilder UseNewContextApplication(this IAppBuilder app, IUnityContainer container)
        {
            RunStartupClassesAsync(container).Wait();
            return app;
        }

        private static async Task RunStartupClassesAsync(IUnityContainer container)
        {
            var startupClasses = container.ResolveAll<IStartup>();

            foreach (var startup in startupClasses)
            {
                var result = await startup.RunAsync();

                if (result.IsFailure)
                    throw new Exception("Failure starting application: " + result.Error);
            }
        }
    }
}
