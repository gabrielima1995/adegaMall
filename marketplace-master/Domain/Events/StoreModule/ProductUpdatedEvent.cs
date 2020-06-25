using Domain.Entities.StoreModule;
using Framework;

namespace Domain.Events.StoreModule
{
    public class ProductUpdatedEvent : DomainEvent
    {
        public Product Product { get; set; }

        public string StoreSellerId { get; set; }

        public ProductUpdatedEvent(Product product, string storeSellerId)
        {
            Guard.Ensure.NotEmpty(product, nameof(product));
            Guard.Ensure.NotEmpty(storeSellerId, nameof(storeSellerId));

            Product = product;
            StoreSellerId = storeSellerId;
        }
    }
}
