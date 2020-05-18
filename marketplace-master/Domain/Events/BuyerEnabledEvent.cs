using Domain.Entities;

namespace Domain.Events
{
    public class BuyerEnabledEvent : DomainEvent
    {
        public Buyer Buyer { get; }

        public string BuyerId => Buyer.Id;

        public BuyerEnabledEvent(Buyer buyer)
        {
            Buyer = buyer;
        }
    }
}