using Domain.StoreModule.Entities;

namespace Domain.Events.StoreModule
{
    public class RentalShiftAddedEvent : DomainEvent
    {
        public RentalShiftAddedEvent(RentalShift rentalShift)
        {
            RentalShift = rentalShift;
        }

        public RentalShift RentalShift { get; }
    }
}
