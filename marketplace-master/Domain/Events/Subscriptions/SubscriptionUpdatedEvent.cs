using System;

namespace Domain.Events.Subscriptions
{
    public class SubscriptionUpdatedEvent : DomainEvent
    {
        public string PersonId { get; private set; }

        public Guid PlanId { get; private set; }

        public string PlanName { get; private set; }

        public SubscriptionUpdatedEvent(string personId, Guid planId, string planName)
        {
            PersonId = personId;
            PlanId = planId;
            PlanName = planName;
        }
    }
}
