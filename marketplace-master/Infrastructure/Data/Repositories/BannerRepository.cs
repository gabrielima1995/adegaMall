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
        public BannerRepository(ScaffoldContext context)
            : base(context)
        {
        }

        public List<Banner> ListActive()
        {
            var cache = new CacheDefinition("__BannerRepository_ListActive", new TimeSpan(0, 30, 0));

            return CacheHelper.List(Context.Banners.Where(
                x => !x.Deleted &&
                (x.StartsAt == null || x.StartsAt <= DateTime.UtcNow) &&
                (x.ExpiresAt == null || x.ExpiresAt >= DateTime.UtcNow)), cache);
        }

        public List<Banner> ListForAdmin()
        {
            return CacheHelper.List(Context.Banners
                .Where(x => !x.Deleted)
                .OrderBy(x => x.StartsAt)
                .OrderByDescending(x => x.ExpiresAt));
        }
    }
}
