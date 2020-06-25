using Domain.StoreModule.Entities;

namespace Domain.Events.StoreModule
{
    public class RentalShiftRemovedEvent : DomainEvent
    {
        public RentalShiftRemovedEvent(RentalShift rentalShift)
        {
            RentalShift = rentalShift;
        }

        public RentalShift RentalShift { get; }
    }
}
