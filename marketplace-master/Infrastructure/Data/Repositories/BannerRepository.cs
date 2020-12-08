using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class BannerRepository : EfCrudRepository<Banner>, IBannerRepository
    {
        private const string LIST_ACTIVE_CACHE_KEY = "__BannerRepository_ListActive";

        public BannerRepository(ScaffoldContext context)
            : base(context)
        {
        }

        public List<Banner> ListActive()
        {
            var cache = new CacheDefinition(LIST_ACTIVE_CACHE_KEY, new TimeSpan(0, 15, 0));

            return CacheHelper.List(Context.Banners
                .AsNoTracking()
                .Where(x => !x.Deleted &&
                            (x.StartsAt == null || x.StartsAt <= DateTime.UtcNow) &&
                            (x.ExpiresAt == null || x.ExpiresAt >= DateTime.UtcNow))
                .OrderBy(i => i.Order)
                .ThenByDescending(i => i.CreatedAt), cache);
        }

        public List<Banner> ListForAdmin()
        {
            return CacheHelper.List(Context.Banners
                .Where(x => !x.Deleted)
                .OrderBy(i => i.Order)
                .ThenByDescending(i => i.CreatedAt));
        }

        public void PurgeCache()
        {
            CacheHelper.Evict(LIST_ACTIVE_CACHE_KEY);
        }
    }
}
