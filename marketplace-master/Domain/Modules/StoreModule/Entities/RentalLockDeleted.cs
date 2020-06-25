using Domain.Events;
using Domain.StoreModule.Entities;

namespace Domain.Modules.StoreModule.Entities
{
    public class RentalLockDeleted : DomainEvent
    {
        public RentalLockDeleted(RentalLock rentalLock)
        {
            RentalLock = rentalLock;
        }

        public RentalLock RentalLock { get; }
    }
}