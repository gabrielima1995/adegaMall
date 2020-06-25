using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class RentalLockRepository : EfCrudRepository<RentalLock>, IRentalLockRepository
    {
        public RentalLockRepository(ScaffoldContext context)
            : base(context)
        {
        }

        public List<RentalLock> GetDayOfWeeksLocks(string sellerId, Guid? productId)
        {
            return Query()
                .Where(i => i.Seller_Id == sellerId && i.Type == LockType.ByDayOfWeek && !i.Deleted)
                .Where(i => i.Product_Id == productId)
                .OrderBy(i => i.StartTime)
                .ThenBy(i => i.DayOfWeek)
                .ToList();
        }
    }
}