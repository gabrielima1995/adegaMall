using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class PortfolioSubCategoryRepository : EfCrudRepository<SubCategory>, IPortfolioSubCategoryRepository
    {
        public PortfolioSubCategoryRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<SubCategory> ListActive(int? categoryId = null)
        {
            var cache = new CacheDefinition($"__PortfolioSubCategoryRepository_ListActive_{categoryId}", new TimeSpan(0, 5, 0));

            return CacheHelper.List(Context.Set<SubCategory>()
                .Include(x => x.Category)
                .Where(x => (!x.Deleted && !x.Category.Deleted) && (categoryId == null || x.Category_Id == categoryId))
                .OrderBy(x => x.Name), cache);
        }
    }
}
