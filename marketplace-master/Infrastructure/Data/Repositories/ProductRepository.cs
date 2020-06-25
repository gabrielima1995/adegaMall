using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Events.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class ProductRepository : EfCrudRepository<Product>, IProductRepository,
        IDomainEventHandler<ProductAddedEvent>,
        IDomainEventHandler<ProductUpdatedEvent>,
        IDomainEventHandler<ProductDeletedEvent>,
        IDomainEventHandler<ProductMovedEvent>,
        IDomainEventHandler<ProductAvailabilityAdded>,
        IDomainEventHandler<ProductAvailabilityUpdated>,
        IDomainEventHandler<ProductAvailabilityDeleted>
    {
        private const string CacheKeyFmt_GetDetailsForPublicPage = "__ProductDetailsForPublicPage_{0}";

        public ProductRepository(ScaffoldContext context) : base(context)
        {
        }

        public bool FriendlyUrlExists(string friendlyUrl)
        {
            return Context.Set<Product>().Any(x => x.FriendlyUrl == friendlyUrl);
        }

        public Product GetByRef(string customId)
        {
            var entity = Context.Set<Product>()
                .FirstOrDefault(x => !x.Deleted
                    && x.CustomId == customId);

            return entity;
        }

        public Product GetDetailsForPublicPage(Guid id, bool requireSellerGatewayId, bool requireSellerActiveMembership)
        {
            var cache = new CacheDefinition(string.Format(CacheKeyFmt_GetDetailsForPublicPage, id), new TimeSpan(0, 30, 0));
            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationEmail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;

            return CacheHelper.Get(Context.Set<Product>()
                .Include(x => x.Images)
                .Include(x => x.Category)
                .Include(x => x.PrivateFleetParams)
                .Include(x => x.Store)
                .Include(x => x.Store.Seller)
                .Include(x => x.Store.Seller.Subscription)
                .Include(x => x.Store.Seller.Address)
                .Include(x => x.Variants)
                .Include(x => x.Variants.Select(v => v.Type))
                .Include(x => x.StockVariantType)
                .Include(x => x.StockVariants)
                .Include(x => x.StockVariants.Select(v => v.Variant))
                .Include(x => x.Availabilities)
                .Where(x =>
                    x.Id == id
                    && x.Deleted == false
                    && (!MaaSConfig.Current.Product.CanPauseProducts || x.IsDisabled == null || !x.IsDisabled.Value)
                    && x.Store.Seller.Enabled
                    && (!requireSellerGatewayId || x.Store.Seller.SupplierGatewayId != null)
                    && (!requireSellerActiveMembership || (undergoingValidation && x.Store.Seller.Email != validationEmail)
                        || (x.Store.Seller.Subscription != null
                        && (x.Store.Seller.Subscription.Status == SubscriptionStatus.Trialing || x.Store.Seller.Subscription.Status == SubscriptionStatus.Paid || x.Store.Seller.Subscription.Status == SubscriptionStatus.PendingPayment)))), cache);
        }

        public Product GetForCartOperations(Guid id)
        {
            return CacheHelper.Get(Context.Set<Product>()
                .Include(x => x.PrivateFleetParams)
                .Include(x => x.Category)
                .Include(x => x.Store)
                .Include(x => x.Store.SingleFreightParams)
                .Include(x => x.Store.Seller)
                .Include(x => x.Store.Seller.Address)
                .Include(x => x.StockVariantType)
                .Include(x => x.StockVariants)
                .Include(x => x.StockVariants.Select(j => j.Variant))
                .Where(x => x.Id == id));
        }

        public List<KeyValuePair<Guid, string>> ListCustomIds(List<Guid> productIds)
        {
            var list = CacheHelper.List(Context.Set<Product>()
                .Select(x => new { x.Id, x.CustomId })
                .Where(x => productIds.Contains(x.Id)), null);

            return list
                .Select(x => new KeyValuePair<Guid, string>(x.Id, x.CustomId))
                .ToList();
        }

        public List<Product> ListProductsByExternalIds(params string[] externalIds)
        {
            if (externalIds == null || externalIds.Length == 0)
                return new List<Product>(0);

            var products = new List<Product>(externalIds.Length);
            var dbLimit = 250;

            while (true)
            {
                var batch = externalIds.Take(dbLimit);
                if (batch.Count() == 0) break;

                products.AddRange(CacheHelper.List(Context.Set<Product>().Where(x =>
                   batch.Any(id => x.CustomId == id)
                   && x.Deleted == false)));

                externalIds = externalIds.Skip(dbLimit).ToArray();
            }

            return products;
        }

        public List<Product> ListForEcommerceHome(int productCategoryId, bool requireSellerGatewayId, bool requireSellerActiveMembership)
        {
            var cache = new CacheDefinition($"__ProductRepository_ListForEcommerceHome_{productCategoryId}_{requireSellerGatewayId}_{requireSellerActiveMembership}", new TimeSpan(0, 5, 0));

            return CacheHelper.List(Context.Set<Product>()
                .Include(x => x.Store)
                .Include(x => x.Store.Seller)
                .Include(x => x.Variants)
                .Where(x =>
                    x.Category_Id == productCategoryId
                    && x.CurrentStock > 0
                    && x.Deleted == false
                    && (!MaaSConfig.Current.Product.CanPauseProducts || x.IsDisabled == null || x.IsDisabled == false)
                    && (x.IsBillingProduct == null || !x.IsBillingProduct.Value)
                    && x.Store.Seller.Enabled
                    && (!requireSellerGatewayId || x.Store.Seller.SupplierGatewayId != null)
                    && (!MaaSConfig.Current.Seller.Moderation.Enabled || x.Store.Seller.RequireModeration != true)
                    && (!requireSellerActiveMembership || (x.Store.Seller.Subscription != null
                        && (x.Store.Seller.Subscription.Status == SubscriptionStatus.Trialing || x.Store.Seller.Subscription.Status == SubscriptionStatus.Paid || x.Store.Seller.Subscription.Status == SubscriptionStatus.PendingPayment))))
                .OrderByDescending(x => x.Store.Seller.Score)
                .Take(16), cache);
        }

        public List<Product> ListTopForSeller(string sellerId)
        {
            var cache = new CacheDefinition($"__TopProductsForSellerQuery_{sellerId}", new TimeSpan(0, 5, 0));

            return CacheHelper.List(Context.Set<Product>()
                .Include(x => x.Category)
                .Where(x =>
                    x.Deleted == false
                    && x.Store_SellerId == sellerId
                    && (!MaaSConfig.Current.Product.CanPauseProducts || x.IsDisabled == null || x.IsDisabled == false)
                    && (x.IsBillingProduct == null || !x.IsBillingProduct.Value)
                    && x.CurrentStock > 0)
                .OrderByDescending(x => x.CreatedAt)
                .Take(16), cache);
        }

        public List<Product> ListByPriceVersionOut()
        {
            return Context.Set<Product>()
                .Where(i => i.ProductPriceVersion != ProductPrice.CurrentVersion && i.Price >= 0)
                .ToList();
        }

        public List<Product> ListByPrice()
        {
            return Context.Set<Product>()
                .Where(i => i.Price > 0)
                .ToList();
        }

        public List<Product> ListByScheduleTimeline(Guid? productId, string sellerId)
        {
            return Context.Set<Product>()
                .Include(i => i.Store.Seller)
                .Where(i => productId == null || i.Id == productId)
                .Where(i => i.Store_SellerId == sellerId)
                .Where(i => !i.Deleted)
                .Where(i => i.IsBillingProduct == null || i.IsBillingProduct == false)
                .ToList();
        }

        #region Event handlers

        public void Handle(ProductAddedEvent domainEvent)
        {
            ClearCache(domainEvent.Product.Id);
        }

        public void Handle(ProductUpdatedEvent domainEvent)
        {
            ClearCache(domainEvent.Product.Id);
        }

        public void Handle(ProductDeletedEvent domainEvent)
        {
            ClearCache(domainEvent.ProductId);
        }

        public void Handle(ProductMovedEvent domainEvent)
        {
            ClearCache(domainEvent.Product.Id);
        }

        private void ClearCache(Guid productId)
        {
            var cacheKey = string.Format(CacheKeyFmt_GetDetailsForPublicPage, productId);
            CacheHelper.Evict(cacheKey);
        }

        #endregion Event handlers

        public void Handle(ProductAvailabilityAdded domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product.Id);
        }

        public void Handle(ProductAvailabilityUpdated domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product.Id);
        }

        public void Handle(ProductAvailabilityDeleted domainEvent)
        {
            ClearCache(domainEvent.ProductAvailability.Product.Id);
        }
    }
}