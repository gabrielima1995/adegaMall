using Framework;
using System;

namespace Domain.Events.StoreModule
{
    public class ProductDeletedEvent : DomainEvent
    {
        public Guid ProductId { get; set; }

        public string StoreSellerId { get; set; }

        public ProductDeletedEvent(Guid productId, string storeSellerId)
        {
            Guard.Ensure.NotEmpty(productId, nameof(productId));
            Guard.Ensure.NotEmpty(storeSellerId, nameof(storeSellerId));

            ProductId = productId;
            StoreSellerId = storeSellerId;
        }
    }
}
