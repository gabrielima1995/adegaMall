using System;

namespace Domain.Events.QuoteRequests
{
    public class QuoteClosedEvent : DomainEvent
    {
        public Guid QuoteId { get; private set; }

        public bool DealWon { get; private set; }

        public QuoteClosedEvent(Guid quoteId, bool dealWon)
        {
            QuoteId = quoteId;
            DealWon = dealWon;
        }
    }
}
