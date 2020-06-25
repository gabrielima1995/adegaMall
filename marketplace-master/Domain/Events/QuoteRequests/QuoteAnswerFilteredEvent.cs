using System;

namespace Domain.Events.QuoteRequests
{
    public class QuoteAnswerFilteredEvent : DomainEvent
    {
        public Guid QuoteId { get; private set; }

        public string BuyerId { get; private set; }

        public string SellerId { get; private set; }

        public string AuthorId { get; private set; }

        public string Message { get; private set; }

        public bool AuthorIsBuyer => AuthorId == BuyerId;

        public bool AuthorIsSeller => AuthorId == SellerId;

        public QuoteAnswerFilteredEvent(Guid quoteId, string sellerId, string buyerId, string authorId, string message)
        {
            QuoteId = quoteId;
            SellerId = sellerId;
            BuyerId = buyerId;
            AuthorId = authorId;
            Message = message;
        }
    }
}