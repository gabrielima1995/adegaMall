using Unity;
using V2.Application.Modules.Sellers;
using V2.Infrastructure.Modules.SellerProfileModule;
using V2.Infrastructure.Modules.SellerTransfer;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    public class SellerScheduledTransferDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
               .RegisterType<ISellerScheduledTransferQueries, SellerScheduledTransfer>();
        }
    }
}
