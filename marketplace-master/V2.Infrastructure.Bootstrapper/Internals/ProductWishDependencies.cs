using Domain.Modules.ProductWishModule;
using Unity;
using V2.Application.Modules.ProductWishModule;
using V2.Infrastructure.Modules.ProductWishModule;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class ProductWishDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<ProductWishAppService>()
                .RegisterType<IProductWishService, ProductWishService>()
                .RegisterType<IProductWishRepository, ProductWishRepository>();
        }
    }
}