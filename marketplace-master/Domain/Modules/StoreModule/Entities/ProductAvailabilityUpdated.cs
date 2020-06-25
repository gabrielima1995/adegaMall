using Domain.Events;

namespace Domain.Modules.StoreModule.Entities
{
    public class ProductAvailabilityUpdated : DomainEvent
    {
        public ProductAvailabilityUpdated(ProductAvailability productAvailability)
        {
            ProductAvailability = productAvailability;
        }

        public ProductAvailability ProductAvailability { get; }
    }
}