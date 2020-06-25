using Domain.Entities.SubscriptionModule;
using Framework;

namespace Domain.Modules.Subscriptions
{
    public class SubscriptionService : ServiceBase<Subscription>
    {
        public SubscriptionService(
            IRepository<Subscription> repository)
            : base(repository)
        {
        }
    }
}