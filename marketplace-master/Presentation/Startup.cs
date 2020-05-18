using Microsoft.Owin;
using Owin;
using Presentation.Configuration;
using System.Web.Mvc;
using Unity;

[assembly: OwinStartup(typeof(Presentation.Startup))]
namespace Presentation
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.GetConfiguredContainer();
            ConfigureServices(container);
            Configure(app, container);
        }

        private void ConfigureServices(IUnityContainer container)
        {
            container
                .AddIdentity()
                .AddInfrastructure()
                .AddNewContext();
        }

        private void Configure(IAppBuilder app, IUnityContainer container)
        {
            app
                .UseAuthentication()
                .UseMaasConfig()
                .UseApplicationV2()
                .UseSmartSearch()
                .UseNewContext()
                .LoadStrings(container);
        }
    }
}