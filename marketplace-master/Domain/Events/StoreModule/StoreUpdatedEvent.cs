using Framework;

namespace Domain.Events.StoreModule
{
    public class StoreUpdatedEvent : DomainEvent
    {
        public string SellerId { get; private set; }

        public StoreUpdatedEvent(string sellerId)
        {
            Guard.Ensure.NotEmpty(sellerId, nameof(sellerId));
            SellerId = sellerId;
        }
    }
}
