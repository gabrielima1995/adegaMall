using System;

namespace Domain.Events.QuoteRequests
{
    public class QuoteClosedByBuyerEvent : DomainEvent
    {
        public Guid QuoteId { get; private set; }

        public QuoteClosedByBuyerEvent(Guid quoteId)
        {
            QuoteId = quoteId;
        }
    }
}
