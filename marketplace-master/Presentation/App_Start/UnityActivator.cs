using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Unity.AspNet.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.UnityActivator), nameof(Presentation.UnityActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Presentation.UnityActivator), nameof(Presentation.UnityActivator.Shutdown))]
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.UnityMvcActivator), nameof(Presentation.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Presentation.UnityMvcActivator), nameof(Presentation.UnityMvcActivator.Shutdown))]

namespace Presentation
{
    public static class UnityActivator
    {
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.AspNet.WebApi.UnityDependencyResolver(container);
        }

        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }

    public static class UnityMvcActivator
    {
        public static void Start()
        {
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.GetConfiguredContainer()));
            DependencyResolver.SetResolver(new UnityDependencyResolver(UnityConfig.GetConfiguredContainer()));
        }

        public static void Shutdown()
        {
            UnityConfig.GetConfiguredContainer().Dispose();
        }
    }
}