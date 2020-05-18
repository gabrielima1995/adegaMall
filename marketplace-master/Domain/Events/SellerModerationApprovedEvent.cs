using Domain.Entities;

namespace Domain.Events
{
    public class SellerModerationApprovedEvent : DomainEvent
    {
        public Seller Seller { get; }

        public string SellerId => Seller.Id;

        public SellerModerationApprovedEvent(Seller seller)
        {
            Seller = seller;
        }
    }
}