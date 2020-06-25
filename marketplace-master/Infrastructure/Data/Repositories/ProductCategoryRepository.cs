using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class ProductCategoryRepository : EfCrudRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<ProductCategory> ListActive()
        {
            var cache = new CacheDefinition("__ProductCategoryRepository_ListActive", new TimeSpan(0, 5, 0));
            return CacheHelper.List(Context.ProductCategories.Where(x => !x.Deleted).OrderBy(x => x.Name), cache);
        }
    }
}
