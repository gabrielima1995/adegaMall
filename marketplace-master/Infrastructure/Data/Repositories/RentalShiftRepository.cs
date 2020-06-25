using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class RentalShiftRepository : IRentalShiftRepository
    {
        private readonly ScaffoldContext scaffoldContext;

        public RentalShiftRepository(ScaffoldContext scaffoldContext)
        {
            this.scaffoldContext = scaffoldContext;
        }

        public OperationResult AddRentalShift(RentalShift rentalShift)
        {
            scaffoldContext.RentalShifts.Add(rentalShift);
            scaffoldContext.SaveChanges();
            return OperationResult.Success;
        }

        public RentalShift Get(Guid id)
        {
            return scaffoldContext.RentalShifts.Find(id);
        }

        public OperationResult RemoveRentalShift(RentalShift rentalShift)
        {
            rentalShift.Deleted = true;
            rentalShift.DeletedAt = DateTime.UtcNow;

            scaffoldContext.Entry(rentalShift).State = System.Data.Entity.EntityState.Modified;
            scaffoldContext.SaveChanges();
            return OperationResult.Success;
        }

        public OperationResult SetProductRentalShifts(Guid productId, IEnumerable<Guid> rentalShiftIds)
        {
            var currentEntries = scaffoldContext.ProductRentalShifts.Where(shift => shift.ProductId == productId).ToArray();
            var currentIds = currentEntries.Select(shift => shift.RentalShiftId).ToArray();

            var mustAddIds = rentalShiftIds.Except(currentIds);
            var mustDeleteIds = currentIds.Except(rentalShiftIds);

            if (mustAddIds.Any())
            {
                var newEntries = mustAddIds.Select(id => ProductRentalShift.Create(productId, id).Result);
                scaffoldContext.ProductRentalShifts.AddRange(newEntries);
            }

            if (mustDeleteIds.Any())
            {
                var deleteEntries = currentEntries.Where(entry => mustDeleteIds.Contains(entry.RentalShiftId));
                scaffoldContext.ProductRentalShifts.RemoveRange(deleteEntries);
            }

            scaffoldContext.SaveChanges();
            return OperationResult.Success;
        }

        public OperationResult UpdateRentalShift(RentalShift rentalShift)
        {
            scaffoldContext.Entry(rentalShift).State = System.Data.Entity.EntityState.Modified;
            scaffoldContext.SaveChanges();
            return OperationResult.Success;
        }
    }
}
