using System;

namespace Domain.Events.Subscriptions
{
    public class SubscriptionAddedEvent : DomainEvent
    {
        public string PersonId { get; private set; }

        public Guid PlanId { get; private set; }

        public string PlanName { get; private set; }

        public SubscriptionAddedEvent(string person, Guid planId, string planName)
        {
            PersonId = person;
            PlanId = planId;
            PlanName = planName;
        }
    }
}
