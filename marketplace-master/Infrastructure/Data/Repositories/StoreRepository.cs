using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Events.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Entities;
using Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Infrastructure.Data.Repositories
{
    public class StoreRepository : EfCrudRepository<Store>, IStoreRepository,
        IDomainEventHandler<ProductAddedEvent>,
        IDomainEventHandler<ProductDeletedEvent>,
        IDomainEventHandler<ProductUpdatedEvent>,
        IDomainEventHandler<StoreUpdatedEvent>,
        IDomainEventHandler<ProductAvailabilityAdded>,
        IDomainEventHandler<ProductAvailabilityUpdated>,
        IDomainEventHandler<ProductAvailabilityDeleted>
    {
        private const string CacheKeyFmt_GetForPublicPage = "__StoreRepository_GetForPublicPage_{0}";

        public StoreRepository(ScaffoldContext context) : base(context)
        {
        }

        public Store GetForEditing(string sellerId)
        {
            //var cache = new CacheDefinition(string.Format(CacheKeyFmt_GetForPublicPage, sellerId), new TimeSpan(0, 30, 0));

            var result = CacheHelper.Get(Context.Set<Store>()
                .IncludeFilter(x => x.Seller)
                .IncludeFilter(x => x.SingleFreightParams)
                .IncludeFilter(x => x.Products.Where(p => !p.Deleted && (p.IsBillingProduct == null || !p.IsBillingProduct.Value)))
                .IncludeFilter(x => x.Products.Where(p => !p.Deleted).Select(p => p.Category))
                .Where(x => x.SellerId == sellerId) /*, cache - CACHE DESATIVADO ATÉ CADASTRO DA LOJA SER COMPLETAMENTE FEITO PELA NOVA CAMADA DE SERVIÇOS */);

            if (result != null)
                result.Products = result.Products.OrderBy(x => x.Title).ToList();

            return result;
        }

        public List<Product> ListByStore(string sellerId)
        {
            return Context.Set<Product>()
                .Where(x => x.Store_SellerId == sellerId && !x.Deleted)
                .ToList();
        }

        public void Add(Store store)
        {
            Context.Set<Store>().Add(store);
        }

        public bool ExistsBySellerId(string sellerId)
        {
            return Context.Set<Store>().Any(x => x.SellerId == sellerId);
        }

        public void Handle(ProductAddedEvent domainEvent)
        {
            ClearCache(domainEvent.StoreSellerId);
        }

        public void Handle(ProductDeletedEvent domainEvent)
        {
            ClearCache(domainEvent.StoreSellerId);
        }

        public void Handle(ProductUpdatedEvent domainEvent)
        {
            ClearCache(domainEvent.StoreSellerId);
        }

        public void Handle(StoreUpdatedEvent domainEvent)
        {
            ClearCache(domainEvent.SellerId);
        }

        private void ClearCache(string sellerId)
        {
            CacheHelper.Evict(string.Format(CacheKeyFmt_GetForPublicPage, sellerId));
        }

        public void Handle(ProductAvailabilityAdded domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product?.Store_SellerId);
        }

        public void Handle(ProductAvailabilityUpdated domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product?.Store_SellerId);
        }

        public void Handle(ProductAvailabilityDeleted domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product?.Store_SellerId);
        }
    }
}