using Domain.Entities.StoreModule;

namespace Domain.Events
{
    public class ProductMovedEvent : DomainEvent
    {
        public Order Order { get; set; }

        public Product Product { get; set; }

        public decimal Amount { get; set; }

        public StockTransactionType Type { get; set; }

        public ProductVariant Variant { get; set; }

        public ProductMovedEvent(Product product, ProductVariant variant, decimal amount, StockTransactionType type)
        {
            Product = product;
            Variant = variant;
            Amount = amount;
            Type = type;
        }

        public ProductMovedEvent(Order order, Product product, ProductVariant variant, decimal amount, StockTransactionType type)
            : this(product, variant, amount, type)
        {
            Order = order;
        }
    }
}
