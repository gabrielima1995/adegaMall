using Domain.Events;
using Domain.Events.QuoteRequests;

namespace Infra.SmartSearchAdapter.Quotes.EventHandlers
{
    public class RemoveQuoteFromIndexWhenQuoteIsClosed : IDomainEventHandler<QuoteClosedByBuyerEvent>
    {
        public void Handle(QuoteClosedByBuyerEvent domainEvent)
        {
            var quoteId = domainEvent.QuoteId;
            new QuoteIndexHelper().RemoveItem(quoteId.ToString());
        }
    }
}