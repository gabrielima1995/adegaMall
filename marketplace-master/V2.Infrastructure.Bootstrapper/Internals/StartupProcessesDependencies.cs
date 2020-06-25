using Unity;
using V2.Application.StartupProcesses;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class StartupProcessesDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<FriendlyUrlBuilder>()
                .RegisterType<SlugifyProductsBuilder>();
        }
    }
}
