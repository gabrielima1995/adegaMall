using Domain.Entities.StoreModule;

namespace Domain.Events
{
    public class ProductCurrentStockChanged : DomainEvent
    {
        public Product Product { get; }

        public ProductCurrentStockChanged(Product product)
        {
            Product = product;
        }
    }
}