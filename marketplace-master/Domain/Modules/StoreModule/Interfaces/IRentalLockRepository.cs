using Domain.Interfaces.Repositories;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.StoreModule.Interfaces
{
    public interface IRentalLockRepository : ICrudRepository<RentalLock>
    {
        List<RentalLock> GetDayOfWeeksLocks(string sellerId, Guid? productId);
    }
}
