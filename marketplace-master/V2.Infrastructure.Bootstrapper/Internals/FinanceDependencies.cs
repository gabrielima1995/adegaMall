using Application.Sellers;
using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Unity;
using V2.Application.Modules.Finance;
using V2.Application.Modules.Finance.Queries;
using V2.Infrastructure.Modules.Finance.Queries;
using V2.Infrastructure.Modules.Finance.Repositories;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class FinanceDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container // Domain.Finance
                .RegisterType<IGatewayRegistrationLogQueries, GatewayRegistrationLogQueries>()
                .RegisterType<IGatewayRegistrationLogRepository, GatewayRegistrationLogRepository>()
                .RegisterType<RegisterLogFromSellerDataCommandHandler>()
                .RegisterType<InitGatewayRegistrationLogJob>();

            container // Application
                .RegisterType<UpdateSellerGatewayCommandHandler>();

            container // V2.Application
                .RegisterType<DoTransfersRecurrentJob>()
                .RegisterType<TransferJournalService>()
                .RegisterType<TransferJournalAppService>()
                .RegisterType<ITransferJournalRepository, TransferJournalRepository>()
                .RegisterType<ITransferJournalQueries, TransferJournalQueries>();
        }
    }
}