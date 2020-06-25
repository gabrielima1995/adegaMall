using System;

namespace Domain.Events.QuoteRequests
{
    public class NewQuoteGroupRegisteredEvent : DomainEvent
    {
        public Guid GroupId { get; private set; }

        public string BuyerId { get; private set; }

        public string Title { get; private set; }

        public bool HasSeller { get; set; }

        public NewQuoteGroupRegisteredEvent(Guid groupId, string buyerId, string title, bool hasSeller = true)
        {
            GroupId = groupId;
            BuyerId = buyerId;
            Title = title;
            HasSeller = hasSeller;
        }
    }
}
