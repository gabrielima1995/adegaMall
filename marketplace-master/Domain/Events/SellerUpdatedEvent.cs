using Domain.Entities;

namespace Domain.Events
{
    public class SellerUpdatedEvent : DomainEvent
    {
        public Seller Seller { get; }

        public string SellerId => Seller.Id;

        public SellerUpdatedEvent(Seller seller)
        {
            Seller = seller;
        }
    }
}