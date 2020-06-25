using NewContext.Acl;
using NewContext.Application;
using NewContext.Infrastructure;
using NewContext.Queries;
using Owin;
using System.Configuration;
using Unity;

namespace Presentation.Configuration
{
    public static class NewContextConfigurationExtensions
    {
        public static IUnityContainer AddNewContext(this IUnityContainer container)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ScaffoldContext"].ConnectionString;

            container
                .AddNewContextInfrastructure(connectionString)
                .AddNewContextQueries(connectionString)
                .AddNewContextAcl(connectionString)
                .AddNewContextApplication();

            return container;
        }

        public static IAppBuilder UseNewContext(this IAppBuilder app)
        {
            var container = UnityConfig.GetConfiguredContainer();

            app
                .UseNewContextInfrastructure(container)
                .UseNewContextApplication(container);

            return app;
        }
    }
}