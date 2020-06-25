using Domain.StoreModule.Entities;

namespace Domain.Events.StoreModule
{
    public class RentalShiftUpdatedEvent : DomainEvent
    {
        public RentalShiftUpdatedEvent(RentalShift rentalShift)
        {
            RentalShift = rentalShift;
        }

        public RentalShift RentalShift { get; }
    }
}
