using System;

namespace Domain.Events.QuoteRequests
{
    public class NewQuoteRegisteredEvent : DomainEvent
    {
        public Guid QuoteId { get; private set; }

        public string BuyerId { get; private set; }

        public string SellerId { get; private set; }

        public string Title { get; private set; }

        public Guid? GroupId { get; private set; }

        public NewQuoteRegisteredEvent(Guid quoteId, string buyerId, string sellerId, string title, Guid? groupId)
        {
            QuoteId = quoteId;
            BuyerId = buyerId;
            SellerId = sellerId;
            Title = title;
            GroupId = groupId;
        }
    }
}
