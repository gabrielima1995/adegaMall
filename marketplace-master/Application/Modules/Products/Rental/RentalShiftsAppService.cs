using Domain.StoreModule.Commands;
using Framework;

namespace Application.Modules.Products.Rental
{
    public class RentalShiftsAppService
    {
        private readonly AddRentalShiftCommandHandler addRentalShiftCommandHandler;
        private readonly RemoveRentalShiftCommandHandler removeRentalShiftCommandHandler;
        private readonly SetProductRentalShiftsCommandHandler setProductRentalShiftsCommandHandler;
        private readonly UpdateRentalShiftCommandHandler updateRentalShiftCommandHandler;

        public RentalShiftsAppService(
            AddRentalShiftCommandHandler addRentalShiftCommandHandler,
            RemoveRentalShiftCommandHandler removeRentalShiftCommandHandler,
            SetProductRentalShiftsCommandHandler setProductRentalShiftsCommandHandler,
            UpdateRentalShiftCommandHandler updateRentalShiftCommandHandler)
        {
            this.addRentalShiftCommandHandler = addRentalShiftCommandHandler;
            this.removeRentalShiftCommandHandler = removeRentalShiftCommandHandler;
            this.setProductRentalShiftsCommandHandler = setProductRentalShiftsCommandHandler;
            this.updateRentalShiftCommandHandler = updateRentalShiftCommandHandler;
        }

        public OperationResult AddRentalShift(AddRentalShiftCommand command)
            => addRentalShiftCommandHandler.Handle(command);

        public OperationResult RemoveRentalShift(RemoveRentalShiftCommand command)
            => removeRentalShiftCommandHandler.Handle(command);

        public OperationResult SetProductRentalShifts(SetProductRentalShiftsCommand command)
            => setProductRentalShiftsCommandHandler.Handle(command);

        public OperationResult UpdateRentalShift(UpdateRentalShiftCommand command)
            => updateRentalShiftCommandHandler.Handle(command);
    }
}
