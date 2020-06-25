using Infrastructure;
using Unity;
using V2.Infrastructure.Bootstrapper;

namespace Presentation.Configuration
{
    public static class InfraBoostrapperConfigurationExtensions
    {
        public static IUnityContainer AddInfrastructure(this IUnityContainer container)
        {
            UnityIocContainer.Instance = container;
            InfraBootstrapper.Setup(container);

            return container;
        }
    }
}