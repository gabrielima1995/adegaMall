using Domain.Entities.SubscriptionModule;

namespace Domain.Events.Subscriptions
{
    public class SubscriptionCancelledEvent : DomainEvent
    {
        public Subscription Subscription { get; }

        public SubscriptionCancelledEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}