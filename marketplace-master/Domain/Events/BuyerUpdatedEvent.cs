using Domain.Entities;

namespace Domain.Events
{
    public class BuyerUpdatedEvent : DomainEvent
    {
        public Buyer Buyer { get; }

        public string BuyerId => Buyer.Id;

        public BuyerUpdatedEvent(Buyer buyer)
        {
            Buyer = buyer;
        }
    }
}