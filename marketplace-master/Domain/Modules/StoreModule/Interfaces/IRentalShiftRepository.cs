using Domain.StoreModule.Entities;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Interfaces
{
    public interface IRentalShiftRepository
    {
        OperationResult AddRentalShift(RentalShift rentalShift);

        RentalShift Get(Guid id);

        OperationResult RemoveRentalShift(RentalShift rentalShift);

        OperationResult UpdateRentalShift(RentalShift rentalShift);

        OperationResult SetProductRentalShifts(Guid productId, IEnumerable<Guid> rentalShiftIds);
    }
}
