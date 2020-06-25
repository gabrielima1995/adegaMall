using Domain.Events;
using Domain.StoreModule.Entities;

namespace Domain.Modules.StoreModule.Entities
{
    public class RentalLockAdded : DomainEvent
    {
        public RentalLockAdded(RentalLock rentalLock)
        {
            RentalLock = rentalLock;
        }

        public RentalLock RentalLock { get; }
    }
}