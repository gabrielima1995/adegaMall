using Domain.Entities.StoreModule;

namespace Domain.Events.StoreModule
{
    public class ProductAddedEvent : DomainEvent
    {
        public Product Product { get; private set; }

        public string StoreSellerId { get; private set; }

        public string Title { get; private set; }

        public int InitialStock { get; private set; }

        public ProductAddedEvent(Product product, string storeSellerId, string title, int initialStock)
        {
            Product = product;
            StoreSellerId = storeSellerId;
            Title = title;
            InitialStock = initialStock;
        }
    }
}
