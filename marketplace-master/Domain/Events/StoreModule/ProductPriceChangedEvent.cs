using Framework;
using System;

namespace Domain.Events.StoreModule
{
    public class ProductPriceChangedEvent : DomainEvent
    {
        public Guid ProductId { get; set; }

        public string StoreSellerId { get; set; }

        public ProductPriceChangedEvent(Guid productId, string storeSellerId)
        {
            Guard.Ensure.NotEmpty(productId, nameof(productId));
            Guard.Ensure.NotEmpty(storeSellerId, nameof(storeSellerId));

            ProductId = productId;
            StoreSellerId = storeSellerId;
        }
    }
}