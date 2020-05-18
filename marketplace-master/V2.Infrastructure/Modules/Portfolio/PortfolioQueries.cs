using Application.Config;
using Dapper;
using Domain.DTO;
using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Modules.Portfolio;
using Framework.Geography;
using Infrastructure.Data.Context;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.Portfolio
{
    public class PortfolioQueries : IPortfolioQueries,
        IDomainEventHandler<PortfolioItemUpdatedEvent>
    {
        private static readonly Dictionary<string, Guid> _urlsToIdsTable = new Dictionary<string, Guid>();
        private static readonly object _lock = new object();

        private readonly ScaffoldContext _context;

        public PortfolioQueries(ScaffoldContext context)
        {
            _context = context;
        }

        public IQueryable<SearchResultItem> SearchItems(SearchRequest searchRequest)
        {
            IQueryable<PortfolioItem> queryable = _context.Set<PortfolioItem>();
            queryable.Include(x => x.Portfolio);
            queryable.Include(x => x.Portfolio.Seller);
            queryable.Include(x => x.SubCategories);

            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;

            var useBounds = searchRequest.Bounds != null;
            var useLocation = !useBounds && searchRequest.Location != null;
            var locationRange = searchRequest.LocationRange ?? MaaSConfig.Current.Search.Map.SearchRange;

            var dbBounds = searchRequest.Bounds?.AsDbGeography();
            var dbLocation = useBounds ? searchRequest.Bounds.GetCenter().AsDbGeography() : searchRequest.Location?.AsDbGeography();

            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationMail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;

            var result = queryable.Select(x => new
            {
                x.Id,
                x.CoverImageUrl,
                x.CreatedAt,
                x.Description,
                x.IsVideo,
                x.Price,
                x.SubCategories,
                x.Title,
                x.VideoUrl,
                x.Portfolio.Seller,
                x.Portfolio.Seller.Subscription,
                Location = x.Portfolio.Seller.Address != null ? x.Portfolio.Seller.Address.Location : null,
                Distance = dbLocation != null && x.Portfolio.Seller.Address != null && x.Portfolio.Seller.Address.Location != null ? x.Portfolio.Seller.Address.Location.Distance(dbLocation) : null
            })
            .Where(x => searchRequest.CategoryId == null || x.SubCategories.Any(s => s.Category_Id == searchRequest.SubCategoryId))
            .Where(x => searchRequest.SubCategoryId == null || x.SubCategories.Any(s => s.Id == searchRequest.SubCategoryId))
            .Where(x => !useBounds || (x.Location != null && (x.Location.Intersects(dbBounds) || x.Distance <= locationRange)))
            .Where(x => !useLocation || x.Distance <= locationRange)
            .Where(x => searchRequest.CityAndState == null || x.Seller.Address.CityAndState == searchRequest.CityAndState)
            .Where(x => searchRequest.Query == null ||
                x.Title.Contains(searchRequest.Query) ||
                x.Description.Contains(searchRequest.Query))
            .Where(x => !subscriptionEnabled || (undergoingValidation && x.Seller.Email != validationMail) ||
                   (x.Subscription != null && (
                    x.Subscription.Status == SubscriptionStatus.Trialing ||
                    x.Subscription.Status == SubscriptionStatus.Paid ||
                    x.Subscription.Status == SubscriptionStatus.PendingPayment)))
            .Where(x => x.Seller.Enabled)
            .Select(x => new SearchResultItem()
            {
                CreatedAt = x.CreatedAt,
                Description = x.Description,
                Title = x.Title,
                Subtitle = x.Seller.Name,
                ImageUrl = x.CoverImageUrl,
                Distance = x.Distance,
                Id = x.Id.ToString(),
                Price = x.Price,
                Handle = x.Seller.FriendlyUrl,
                BusinessScore = x.Seller.Score,
                LocationLat = x.Location.Latitude,
                LocationLng = x.Location.Longitude,
            });
            return result;
        }

        public Guid? GetIdByFriendlyUrl(string friendlyUrl)
        {
            if (!_urlsToIdsTable.ContainsKey(friendlyUrl))
            {
                lock (_lock)
                {
                    using (var conn = MyDbConnection.Create())
                    {
                        var sql = "SELECT TOP 1 Id FROM PortfolioItem WHERE FriendlyUrl = @friendlyUrl";
                        var id = conn.ExecuteScalar<Guid?>(sql, new { friendlyUrl });

                        if (id != null && !_urlsToIdsTable.ContainsKey(friendlyUrl))
                            _urlsToIdsTable.Add(friendlyUrl, id.Value);
                    }
                }
            }

            if (_urlsToIdsTable.ContainsKey(friendlyUrl))
                return _urlsToIdsTable[friendlyUrl];

            return null;
        }

        public void Handle(PortfolioItemUpdatedEvent domainEvent)
        {
            foreach (var item in _urlsToIdsTable)
            {
                if (item.Value == domainEvent.PortfolioItem.Id)
                {
                    _urlsToIdsTable.Remove(item.Key);
                    return;
                }
            }
        }
    }
}