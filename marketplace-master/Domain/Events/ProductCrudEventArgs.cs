using System;

namespace Domain.Events
{
    public class ProductCrudEventArgs : EventArgs
    {
        public string SellerId { get; private set; }
        public Guid ProductId { get; private set; }
        public CrudEventType Type { get; private set; }

        public ProductCrudEventArgs(string sellerId, Guid productId, CrudEventType type)
        {
            SellerId = sellerId;
            ProductId = productId;
            Type = type;
        }
    }
}
