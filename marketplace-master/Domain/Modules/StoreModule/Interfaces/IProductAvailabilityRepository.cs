using System;
using System.Collections.Generic;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Entities;

namespace Domain.Modules.StoreModule.Interfaces
{
    public interface IProductAvailabilityRepository : ICrudRepository<ProductAvailability>
    {
        List<ProductAvailability> GetListByProduct(Guid productId, bool onlyWithStock, bool onlyActive, Guid? id);

        List<ProductAvailability> GetListByProduct(Guid productId, DayOfWeek dayOfWeek, bool onlyWithStock);
    }
}