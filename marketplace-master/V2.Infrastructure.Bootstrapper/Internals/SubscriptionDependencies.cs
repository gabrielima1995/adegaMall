using Application.Modules.Subscriptions;
using Application.Modules.Subscriptions.Plans;
using Domain.Interfaces.Repositories;
using Domain.Modules.Subscriptions;
using Domain.Modules.Subscriptions.Plans;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.LeadSales;
using V2.Application.Modules.Subscriptions.Queries;
using V2.Infrastructure.Modules.Subscription.Queries;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class SubscriptionDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<PlanService, PlanService>()
                .RegisterType<SubscriptionService, SubscriptionService>()
                .RegisterType<IPlanAppService, PlanAppService>()
                .RegisterType<IPlanRepository, PlanRepository>()
                .RegisterType<ISubscriptionQueries, SubscriptionQueries>()
                .RegisterType<SubscriptionAppService>()
                .RegisterType<CheckSubscriptionCredits, CheckSubscriptionCredits>();
        }
    }
}