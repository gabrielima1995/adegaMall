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
    public class PortfolioCategoryRepository : EfCrudRepository<Category>, IPortfolioCategoryRepository
    {
        public PortfolioCategoryRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<Category> ListActive()
        {
            var cache = new CacheDefinition("__PortfolioCategoryRepository_ListActive", new TimeSpan(0, 5, 0));

            var results = CacheHelper.List(Context.Set<Category>()
                .Include(x => x.SubCategories)
                .Where(x => !x.Deleted), cache);

            return results.OrderBy(c =>
            {
                c.SubCategories = c.SubCategories.OrderBy(s => s.Name).ToList();
                return c.Name;
            }).ToList();
        }

        public List<Category> ListServicedBySeller(string sellerId)
        {
            var cache = new CacheDefinition($"__PortfolioCategoryRepository_ListServicedBySeller_{sellerId}", new TimeSpan(0, 5, 0));

            var subCategories = CacheHelper.List(Context.Set<SubCategory>()
                .Include(sub => sub.Category)
                .Where(sub => sub.Portfolios.Any(p => p.SellerId == sellerId))
                .Where(sub => !sub.Deleted && !sub.Category.Deleted), cache);

            var categories = subCategories
                .Select(sub => sub.Category)
                .Distinct(new CategoryComparer())
                .OrderBy(cat => cat.Name);

            foreach (var cat in categories)
                cat.SubCategories = subCategories
                    .Where(sub => sub.Category_Id == cat.Id)
                    .OrderBy(sub => sub.Name)
                    .ToList();

            return categories.ToList();
        }

        private class CategoryComparer : IEqualityComparer<Category>
        {
            public bool Equals(Category x, Category y)
                => x != null && y != null && x.Id == y.Id;

            public int GetHashCode(Category obj)
                => obj.Id.GetHashCode();
        }
    }
}
