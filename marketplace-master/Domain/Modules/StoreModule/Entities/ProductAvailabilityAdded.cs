using Domain.Events;

namespace Domain.Modules.StoreModule.Entities
{
    public class ProductAvailabilityAdded : DomainEvent
    {
        public ProductAvailabilityAdded(ProductAvailability productAvailability)
        {
            ProductAvailability = productAvailability;
        }

        public ProductAvailability ProductAvailability { get; }
    }
}