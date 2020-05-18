using Unity;
using V2.Application.Modules.Sellers;
using V2.Application.Modules.StoreModule;
using V2.Infrastructure.Modules.SellerProfileModule;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    public class SellerProfileDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
               .RegisterType<ISellerCompleteProfileQueries, SellerCompleteProfileQueries>()
               .RegisterType<SellerProfileValidationService>();
        }
    }
}
