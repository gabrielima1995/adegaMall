using Domain.Interfaces.Repositories;
using Domain.StoreModule.Interfaces;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.Disputes;
using V2.Application.Modules.Finance;
using V2.Application.Modules.Products.Orders;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class DisputeDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<TransactionWarrantyTransferJobs>()
                .RegisterType<IDisputeAppService, DisputeAppService>()
                .RegisterType<IDisputeRepository, DisputeRepository>()
                .RegisterType<IOrderAppServiceV2, OrderAppServiceV2>()
                .RegisterType<IScheduledTransferRepository, ScheduledTransferRepository>();
        }
    }
}