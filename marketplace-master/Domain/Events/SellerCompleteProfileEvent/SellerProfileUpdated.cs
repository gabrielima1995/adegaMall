namespace Domain.Events.SellerProfileUpdated
{
    public class SellerProfileUpdated : DomainEvent
    {
        public string SellerId { get; protected set; }

        public SellerProfileUpdated(string sellerId)
        {
            SellerId = sellerId;
        }
    }
}
