using Domain.Interfaces.Repositories;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.LeadSales;
using V2.Infrastructure.Modules.LeadSales;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class LeadSalesDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<ILeadSalesAppService, LeadSalesAppService>()
                .RegisterType<ILeadSalesQueries, LeadSalesQueries>()
                .RegisterType<ICreditsPackRepository, CreditsPackRepository>();
        }
    }
}