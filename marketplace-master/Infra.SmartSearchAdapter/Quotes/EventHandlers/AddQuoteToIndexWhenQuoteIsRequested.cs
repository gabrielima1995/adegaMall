using Domain.Events;
using Domain.Events.QuoteRequests;

namespace Infra.SmartSearchAdapter.Quotes.EventHandlers
{
    public class AddQuoteToIndexWhenQuoteIsRequested : IDomainEventHandler<NewQuoteGroupRegisteredEvent>
    {
        public void Handle(NewQuoteGroupRegisteredEvent domainEvent)
        {
            if (!Application.Config.MaaSConfig.Current.Search.QuotesSearch.Enabled)
                return;

            new QuoteIndexHelper().UpdateItem(domainEvent.GroupId);
        }
    }
}