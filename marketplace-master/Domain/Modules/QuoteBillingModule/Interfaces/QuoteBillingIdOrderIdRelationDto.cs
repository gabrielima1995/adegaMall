using System;

namespace Domain.Modules.QuoteBillingModule.Interfaces
{
    public class QuoteBillingIdOrderIdRelationDto
    {
        public Guid QuoteBillingId { get; set; }
        public Guid OrderId { get; set; }
    }

    public class BillingQuoteProductIds
    {
        public Guid QuoteBillingId { get; set; }
        public Guid QuoteId { get; set; }
        public Guid ProductId { get; set; }
    }
}