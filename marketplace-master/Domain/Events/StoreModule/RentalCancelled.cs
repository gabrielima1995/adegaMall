using Domain.Entities.StoreModule;

namespace Domain.Events.StoreModule
{
    public class RentalCancelled : DomainEvent
    {
        public Order Order { get; }

        public RentalCancelled(Order order)
        {
            Order = order;
        }
    }
}