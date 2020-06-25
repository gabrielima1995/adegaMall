using Domain.Modules.QuoteBillingModule.Entities;

namespace Domain.Events.QuoteBillingModule
{
    public class QuoteBillingRefundedEvent : DomainEvent
    {
        public QuoteBilling QuoteBilling { get; private set; }

        public QuoteBillingRefundedEvent(QuoteBilling quoteBilling)
        {
            QuoteBilling = quoteBilling;
        }
    }
}
