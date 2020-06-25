using Domain.Events;

namespace Domain.Modules.StoreModule.Entities
{
    public class ProductAvailabilityDeleted : DomainEvent
    {
        public ProductAvailabilityDeleted(ProductAvailability productAvailability)
        {
            ProductAvailability = productAvailability;
        }

        public ProductAvailability ProductAvailability { get; }
    }
}