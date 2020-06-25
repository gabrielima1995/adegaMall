using Domain.Entities;
using Domain.Events;
using Domain.Events.EcommerceHome;
using Domain.Interfaces.Repositories;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Infrastructure.Data.Repositories
{
    public class EcommerceHomeRepository : EfCrudRepository<EcommerceHome>, IEcommerceHomeRepository,
        IDomainEventHandler<HomeCategoryAddedEvent>,
        IDomainEventHandler<HomeCategoryUpdatedEvent>,
        IDomainEventHandler<HomeCategoryRemovedEvent>
    {
        private const string _getCacheKey = "__EcommerceHomeRepository_Get";

        public EcommerceHomeRepository(ScaffoldContext context) : base(context) { }

        public EcommerceHome Get()
        {
            var cache = new CacheDefinition(_getCacheKey, new TimeSpan(0, 5, 0));

            var result = CacheHelper.Get(Context.EcomerceHome
                .IncludeFilter(x => x.Categories.Where(c => !c.Deleted))
                .IncludeFilter(x => x.Categories.Where(c => !c.Deleted).Select(c => c.ProductCategory))
                .IncludeFilter(x => x.Categories.Where(c => !c.Deleted).Select(c => c.CustomItemCategory)), cache);

            if (result != null)
                result.Categories = result.Categories.OrderBy(x => x.Order).ToList();

            return result;
        }

        public EcommerceHomeCategory GetCategory(Guid ecommerceHomeCategoryId)
        {
            var cache = new CacheDefinition(GetHomeCategoryCacheKey(ecommerceHomeCategoryId), new TimeSpan(0, 5, 0));

            return CacheHelper.Get(Context.EcommerceHomeCategories
                .Include(x => x.ProductCategory)
                .Include(x => x.CustomItemCategory)
                .Where(x => x.Id == ecommerceHomeCategoryId));
        }

        public void Handle(HomeCategoryAddedEvent domainEvent)
        {
            CacheHelper.Evict(_getCacheKey);
        }

        public void Handle(HomeCategoryUpdatedEvent domainEvent)
        {
            var categoryCacheKey = GetHomeCategoryCacheKey(domainEvent.HomeCategoryId);
            CacheHelper.Evict(categoryCacheKey);
            CacheHelper.Evict(_getCacheKey);
        }

        public void Handle(HomeCategoryRemovedEvent domainEvent)
        {
            CacheHelper.Evict(_getCacheKey);
        }

        private string GetHomeCategoryCacheKey(Guid ecommerceHomeCategoryId)
        {
            return string.Format("__EcommerceHomeRepository_GetCategory_{0}", ecommerceHomeCategoryId);
        }
    }
}
