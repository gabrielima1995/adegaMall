using Domain.Entities;

namespace Domain.Events
{
    public class SellerEnabledEvent : DomainEvent
    {
        public Seller Seller { get; }

        public string SellerId => Seller.Id;

        public SellerEnabledEvent(Seller seller)
        {
            Seller = seller;
        }
    }
}