using Domain.Entities;

namespace Domain.Events
{
    public class BuyerModerationApprovedEvent : DomainEvent
    {
        public Buyer Buyer { get; }

        public string BuyerId => Buyer.Id;

        public BuyerModerationApprovedEvent(Buyer buyer)
        {
            Buyer = buyer;
        }
    }
}