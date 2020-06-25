using Domain.Entities.StoreModule;
using Domain.Events;

namespace Application.Modules.BatchImport.Events
{
    public class ProductImportedEvent : DomainEvent
    {
        public ProductImportedEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get; }
    }
}
