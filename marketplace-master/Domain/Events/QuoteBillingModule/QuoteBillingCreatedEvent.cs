using Domain.Modules.QuoteBillingModule.Entities;

namespace Domain.Events.QuoteBillingModule
{
    public class QuoteBillingCreatedEvent : DomainEvent
    {
        public QuoteBilling QuoteBilling { get; private set; }

        public QuoteBillingCreatedEvent(QuoteBilling quoteBilling)
        {
            QuoteBilling = quoteBilling;
        }
    }
}
