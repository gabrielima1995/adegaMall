using Domain.Modules.QuoteBillingModule.Entities;

namespace Domain.Events.QuoteBillingModule
{
    public class QuoteBillingCancelledEvent : DomainEvent
    {
        public QuoteBilling QuoteBilling { get; private set; }

        public QuoteBillingCancelledEvent(QuoteBilling quoteBilling)
        {
            QuoteBilling = quoteBilling;
        }
    }
}
