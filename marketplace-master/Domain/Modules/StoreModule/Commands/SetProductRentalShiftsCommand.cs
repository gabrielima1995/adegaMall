using Domain.Core;
using Domain.StoreModule.Interfaces;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Commands
{
    public class SetProductRentalShiftsCommand : Command
    {
        public SetProductRentalShiftsCommand(Guid productId, IEnumerable<Guid> rentalShiftIds) : base(productId.ToString())
        {
            ProductId = productId;
            RentalShiftIds = rentalShiftIds;
        }

        public Guid ProductId { get; private set; }
        public IEnumerable<Guid> RentalShiftIds { get; private set; }
    }

    public class SetProductRentalShiftsCommandHandler
    {
        private readonly IRentalShiftRepository rentalShiftRepository;

        public SetProductRentalShiftsCommandHandler(IRentalShiftRepository rentalShiftRepository)
        {
            this.rentalShiftRepository = rentalShiftRepository;
        }

        public OperationResult Handle(SetProductRentalShiftsCommand command)
        {
            return rentalShiftRepository.SetProductRentalShifts(
                command.ProductId,
                command.RentalShiftIds);
        }
    }
}
