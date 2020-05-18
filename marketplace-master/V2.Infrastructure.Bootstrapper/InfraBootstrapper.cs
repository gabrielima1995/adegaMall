using Application.Config;
using Infrastructure;
using System;
using Unity;
using V2.Application;
using V2.Infrastructure.Bootstrapper.Internals;

namespace V2.Infrastructure.Bootstrapper
{
    public class InfraBootstrapper
    {
        private static IUnityContainer container;

        public static void Setup(IUnityContainer container)
        {
            InfraBootstrapper.container = container; //.AddExtension(new DecoratorContainerExtension());

            SetupDependencies(container);

            CacheManager.Instance = container.Resolve<CacheManager>();
            MaaSConfig.Loaded += MaaSConfig_Loaded;
        }

        private static void SetupDependencies(IUnityContainer container)
        {
            var interfaceType = typeof(IDependenciesRegistration);
            var concreteTypes = interfaceType.Assembly.GetTypesImplementing(interfaceType);

            foreach (var type in concreteTypes)
            {
                var registration = (IDependenciesRegistration)Activator.CreateInstance(type);
                registration.Register(container);
            }
        }

        private static void MaaSConfig_Loaded(object sender, EventArgs e)
        {
            new PaymentDependencies().SetupAfterConfigLoaded(container);
            ApplicationStartup.DependenciesLoaded = true;
        }
    }
}