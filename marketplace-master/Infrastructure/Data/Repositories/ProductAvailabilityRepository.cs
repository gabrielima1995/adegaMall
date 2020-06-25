using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Interfaces;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class ProductAvailabilityRepository : EfCrudRepository<ProductAvailability>, IProductAvailabilityRepository
    {
        public ProductAvailabilityRepository(ScaffoldContext context)
            : base(context)
        {
        }

        public List<ProductAvailability> GetListByProduct(Guid productId, bool onlyWithStock, bool onlyActive, Guid? id)
        {
            return Query()
                .Where(i => !id.HasValue || i.Type != ProductAvailabilityType.Recurring || i.Id == id)
                .Where(i => i.Product_Id == productId)
                .Where(i => !onlyWithStock || i.CurrentStock > 0)
                .Where(i => !onlyActive || !i.Deleted)
                .OrderBy(i => i.StartTime)
                .ThenBy(i => i.DayOfWeek)
                .ToList();
        }

        public List<ProductAvailability> GetListByProduct(Guid productId, DayOfWeek dayOfWeek, bool onlyWithStock)
        {
            return Query()
                .Where(i => i.Product_Id == productId && i.DayOfWeek == dayOfWeek && !i.Deleted)
                .Where(i => !onlyWithStock || i.CurrentStock > 0)
                .OrderBy(i => i.StartTime)
                .ThenBy(i => i.DayOfWeek)
                .ToList();
        }
    }
}