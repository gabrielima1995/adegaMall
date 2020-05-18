using Domain.Modules.QuoteBillingModule.Interfaces;
using Unity;
using V2.Application.Modules.QuoteBillingModule;
using V2.Application.Modules.QuoteBillingModule.Queries;
using V2.Infrastructure.Modules.QuoteBillingModule.Queries;
using V2.Infrastructure.Modules.QuoteBillingModule.Repositories;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class QuoteBillingDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<QuoteBillingAppService>()
                .RegisterType<IQuoteBillingQueries, QuoteBillingQueries>()
                .RegisterType<IQuoteBillingRepository, QuoteBillingRepository>();
        }
    }
}