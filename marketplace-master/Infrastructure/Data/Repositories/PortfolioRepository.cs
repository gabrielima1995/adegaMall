using Application.Config;
using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Infrastructure.Caching;
using Infrastructure.Data.Context;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Framework;

namespace Infrastructure.Data.Repositories
{
    public class PortfolioRepository : EfCrudRepository<Portfolio>, IPortfolioRepository,
        IDomainEventHandler<PortfolioItemAddedEvent>,
        IDomainEventHandler<PortfolioItemRemovedEvent>,
        IDomainEventHandler<PortfolioItemUpdatedEvent>,
        IDomainEventHandler<PortfolioUpdatedEvent>
    {
        private const string CacheKeyFmt_GetForPublicPage = "__PortfolioRepository_GetForPublicPage_{0}";
        private const string CacheKeyFmt_GetItemForPublicPage = "__PortfolioRepository_GetItemForPublicPage_{0}";
        private const string CacheKeyFmt_GetRelatedForItem = "__PortfolioRepository_GetRelatedForItem_{0}";

        public PortfolioRepository(ScaffoldContext context) : base(context)
        {
        }

        public Portfolio GetForPublicPage(string sellerId)
        {
            var cache = new CacheDefinition(string.Format(CacheKeyFmt_GetForPublicPage, sellerId), new TimeSpan(0, 5, 0));

            var result = CacheHelper.Get(Context.Set<Portfolio>()
                .Include(x => x.Items)
                .Include(x => x.Items.Select(i => i.Images))
                .Include(x => x.Items.Select(i => i.SubCategories))
                .Include(x => x.Items.Select(i => i.SubCategories.Select(s => s.Category)))
                .Include(x => x.SubCategories)
                .Include(x => x.SubCategories.Select(s => s.Category))
                .Where(x => x.SellerId == sellerId), cache);

            if (result != null)
            {
                result.SubCategories = result.SubCategories.Where(x => !x.Deleted).ToList();

                foreach (var item in result.Items)
                    item.SubCategories = item.SubCategories.Where(x => !x.Deleted).ToList();
            }

            return result;
        }

        public IQueryable<PortfolioItem> GetQueryable()
        {
            return Context.Set<PortfolioItem>() as IQueryable<PortfolioItem>;
        }

        public PortfolioItem GetItemForPublicPage(Guid id)
        {
            var cache = new CacheDefinition(string.Format(CacheKeyFmt_GetForPublicPage, id), new TimeSpan(0, 30, 0));

            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;
            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationMail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;

            var result = CacheHelper.Get(Context.Set<PortfolioItem>()
                .Include(x => x.Images)
                .Include(x => x.SubCategories)
                .Include(x => x.SubCategories)
                .Include(x => x.SubCategories.Select(s => s.Category))
                .Include(x => x.Portfolio)
                .Include(x => x.Portfolio.Seller)
                .Where(x => x.Id == id)
                .Where(x => !subscriptionEnabled || (undergoingValidation && x.Portfolio.Seller.Email != validationMail) ||
                            (x.Portfolio.Seller.Subscription != null && (
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.Trialing ||
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.Paid ||
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.PendingPayment)))
                .Where(x => x.Portfolio.Seller.Enabled), cache);

            if (result != null)
            {
                result.SubCategories = result.SubCategories.Where(x => !x.Deleted).ToList();

                if (result.Images.Any())
                {
                    result.Images = result.Images.OrderBy(i => i.Position).ToList();
                }
            }

            return result;
        }

        public List<PortfolioItem> GetRelatedItems(Guid portfolioItemId, IEnumerable<Guid> sellerItemsIds)
        {
            var cache = new CacheDefinition(string.Format(CacheKeyFmt_GetRelatedForItem, portfolioItemId), new TimeSpan(0, 15, 0));

            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;
            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationMail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;

            var portfolioItem = Context
                .Set<PortfolioItem>()
                .Include(i => i.SubCategories)
                .FirstOrDefault(i => i.Id == portfolioItemId);

            var queryable = Context
                .Set<PortfolioItem>()
                .Include(i => i.Images)
                .Include(i => i.SubCategories)
                .Include(i => i.Portfolio.Seller);

            var subCategoriesIds = portfolioItem.SubCategories.Select(i => i.Id).ToArray();

            queryable = queryable
                .Where(x => x.SubCategories.Any(i => subCategoriesIds.Contains(i.Id)));

            return CacheHelper.List(queryable
                .Where(x =>
                    x.Id != portfolioItemId &&
                    x.Portfolio.Seller.Enabled && x.Portfolio.Seller.RequireModeration != true)
                .Where(x => !subscriptionEnabled ||
                            (undergoingValidation && x.Portfolio.Seller.Email != validationMail) ||
                            (x.Portfolio.Seller.Subscription != null && (
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.Trialing ||
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.Paid ||
                                 x.Portfolio.Seller.Subscription.Status == SubscriptionStatus.PendingPayment)))
                .Where(x => !sellerItemsIds.Contains(x.Id))
                .Take(30), cache);
        }

        public bool ExistsByFriendlyUrl(string friendlyUrl, Guid ignoreId)
        {
            var set = Context.Set<PortfolioItem>();

            return set.Any(i => i.FriendlyUrl == friendlyUrl && i.Id != ignoreId);
        }

        public OperationResult Update(PortfolioItem entity, bool saveChanges = false)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                if (saveChanges) return SaveChanges();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult("", ex.Message);
            }
        }

        public Portfolio GetPortfolioForSeller(string sellerId)
        {
            return Context.Set<Portfolio>()
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.SubCategories)
                .Include(x => x.Items)
                .SingleOrDefault();
        }

        #region Event handlers

        public void Handle(PortfolioItemAddedEvent domainEvent)
        {
            ClearCache(domainEvent.SellerId, domainEvent.PortfolioItem.Id);
        }

        public void Handle(PortfolioItemRemovedEvent domainEvent)
        {
            ClearCache(domainEvent.SellerId, domainEvent.PortfolioItem.Id);
        }

        public void Handle(PortfolioItemUpdatedEvent domainEvent)
        {
            ClearCache(domainEvent.SellerId, domainEvent.PortfolioItem.Id);
        }

        public void Handle(PortfolioUpdatedEvent domainEvent)
        {
            ClearCache(domainEvent.Portfolio.SellerId, null);
        }

        private void ClearCache(string sellerId, Guid? portfolioId)
        {
            CacheHelper.Evict(string.Format(CacheKeyFmt_GetForPublicPage, sellerId));

            if (portfolioId.HasValue) CacheHelper.Evict(string.Format(CacheKeyFmt_GetItemForPublicPage, portfolioId));
        }

        #endregion Event handlers
    }
}