using Unity;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal interface IDependenciesRegistration
    {
        void Register(IUnityContainer container);
    }
}