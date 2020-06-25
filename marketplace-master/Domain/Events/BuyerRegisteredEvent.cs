using Domain.Entities;

namespace Domain.Events
{
    public class BuyerRegisteredEvent : DomainEvent
    {
        public string BuyerId => Buyer.Id;

        public Buyer Buyer { get; }

        public BuyerRegisteredEvent(Buyer buyer)
        {
            Buyer = buyer ?? throw new System.ArgumentNullException(nameof(buyer));
        }
    }
}