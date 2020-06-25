using Domain.Modules.QuoteBillingModule.Entities;

namespace Domain.Events.QuoteBillingModule
{
    public class QuoteBillingPaidEvent : DomainEvent
    {
        public QuoteBilling QuoteBilling { get; private set; }

        public QuoteBillingPaidEvent(QuoteBilling quoteBilling)
        {
            QuoteBilling = quoteBilling;
        }
    }
}
