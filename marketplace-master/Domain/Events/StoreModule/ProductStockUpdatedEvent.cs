using Domain.Entities.StoreModule;
using Framework;

namespace Domain.Events.StoreModule
{
    public class ProductStockUpdatedEvent : DomainEvent
    {
        public Product Product { get; private set; }

        public ProductVariant Variant { get; private set; }

        public string Title { get; private set; }

        public int NewStock { get; private set; }

        public int OldStock { get; private set; }

        public ProductStockUpdatedEvent(Product product, ProductVariant variant, string title, int newStock, int oldStock)
        {
            Guard.Ensure.NotEmpty(product, nameof(product));
            Guard.Ensure.NotEmpty(title, nameof(title));
            Guard.Ensure.AreNotEqual(newStock, oldStock, nameof(newStock), "New and old stock values must be different when raising ProductStockUpdatedEvent.");

            Product = product;
            Variant = variant;
            Title = title;
            NewStock = newStock;
            OldStock = oldStock;
        }
    }
}
