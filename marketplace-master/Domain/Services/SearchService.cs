using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Framework;
using Framework.Geography;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Data.Entity;
using System.Linq;

namespace Domain.Services
{
    public class SearchService
    {
        private readonly IRepository<Seller> _sellerRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        public SearchService(
            IRepository<Seller> sellerRepository,
            IPortfolioRepository portfolioRepository)
        {
            _sellerRepository = sellerRepository;
            _portfolioRepository = portfolioRepository;
        }

        public OperationResult<SearchResults> Search(SearchRequest searchRequest)
        {
            if (searchRequest == null) throw new ArgumentNullException(nameof(searchRequest));

            try
            {
                IQueryable<SearchResultItem> items = null;

                switch (searchRequest.Type)
                {
                    case SearchRequestType.Seller:
                        items = SearchSellers(searchRequest);
                        break;

                    case SearchRequestType.Product:
                        items = SearchProducts(searchRequest);
                        break;

                    case SearchRequestType.Portfolio:
                        items = SearchPortfolios(searchRequest);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchRequest.Type), $"Search request type '{searchRequest.Type}' is not recognized.");
                }

                var sortedList = SetSorting(searchRequest, items);
                var results = MapResults(searchRequest, sortedList);

                return results;
            }
            catch (Exception ex)
            {
                return new ErrorDictionary("", ex.Message);
            }
        }

        private SearchResults MapResults(SearchRequest searchRequest, IOrderedQueryable<SearchResultItem> sortedList)
        {
            var allItems = sortedList.ToList();
            var pagedItems = allItems.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToList();

            var result = new SearchResults
            {
                Items = pagedItems,
                Request = searchRequest,
                TotalItems = allItems.Count()
            };

            return result;
        }

        private IQueryable<SearchResultItem> SearchSellers(SearchRequest searchRequest)
        {
            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;

            var paymentEnabled = MaaSConfig.Current.Payment.Enabled && !MaaSConfig.Current.Search.IgnorePaymentRequirements && !subscriptionEnabled;

            var useBounds = searchRequest.Bounds != null;
            var useLocation = !useBounds && searchRequest.Location != null;
            var locationRange = searchRequest.LocationRange ?? MaaSConfig.Current.Search.Map.SearchRange;

            var dbBounds = searchRequest.Bounds?.AsDbGeography();
            var dbLocation = useBounds ? searchRequest.Bounds.GetCenter().AsDbGeography() : searchRequest.Location?.AsDbGeography();

            // Check if point is inside bounds
            // https://stackoverflow.com/questions/13960878/check-if-dbgeometry-dbgeometry-dbgeography-point-is-within-a-polygon

            var requireCategory = MaaSConfig.Current.Portfolio.Enabled && MaaSConfig.Current.Quote.Enabled;
            var undergoingValidation = MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var validationMail = MaaSConfig.Current.Payment.PagarMe.ValidationAccountEmail;

            return _sellerRepository.Query()
                .Select(seller => new
                {
                    seller.Id,
                    seller.AlternativeName,
                    seller.FullName,
                    seller.Nickname,
                    seller.AboutMe,
                    seller.AdditionalInfo2,
                    seller.Avatar,
                    seller.Portfolio.SubCategories,
                    seller.Address,
                    seller.Subscription,
                    seller.CreatedAt,
                    seller.Enabled,
                    seller.FriendlyUrl,
                    seller.Email,
                    seller.SupplierGatewayId,
                    seller.RequireModeration,
                    Score = seller.Quotes.Count(),
                    Location = seller.Address != null ? seller.Address.Location : null,
                    Distance = dbLocation != null && seller.Address != null && seller.Address.Location != null ? seller.Address.Location.Distance(dbLocation) : null
                })
                .Where(x => !requireCategory || x.SubCategories.Any())
                .Where(x => searchRequest.CategoryId == null || x.SubCategories.Any(s => s.Category_Id == searchRequest.CategoryId))
                .Where(x => searchRequest.SubCategoryId == null || x.SubCategories.Any(s => s.Id == searchRequest.SubCategoryId))
                .Where(x => searchRequest.OnlyWithPicture == false || x.Avatar != null)
                .Where(x => !paymentEnabled || (x.SupplierGatewayId != null && x.SupplierGatewayId != ""))

                // If the address is located inside the polygon or close enough to its center.
                .Where(x => !useBounds || (x.Location != null && (x.Location.Intersects(dbBounds) || x.Distance <= locationRange)))

                // If the seller is close enough to the searched location.
                .Where(x => !useLocation || x.Distance <= locationRange)

                .Where(x => searchRequest.CityAndState == null || x.Address.CityAndState == searchRequest.CityAndState)
                .Where(x => searchRequest.Query == null ||
                    x.Nickname.Contains(searchRequest.Query) ||
                    x.FullName.Contains(searchRequest.Query) ||
                    x.AlternativeName.Contains(searchRequest.Query) ||
                    x.AboutMe.Contains(searchRequest.Query))
                .Where(x => !subscriptionEnabled || (undergoingValidation && x.Email != validationMail) ||
                   (x.Subscription != null && (
                    x.Subscription.Status == SubscriptionStatus.Trialing ||
                    x.Subscription.Status == SubscriptionStatus.Paid ||
                    x.Subscription.Status == SubscriptionStatus.PendingPayment)))
                .Where(x => x.Enabled)
                .Where(x => !MaaSConfig.Current.Seller.Moderation.Enabled || x.RequireModeration != true)

                .Select(x => new SearchResultItem
                {
                    Id = x.Id,
                    Handle = x.FriendlyUrl,
                    ImageUrl = x.Avatar,
                    Title = x.AlternativeName ?? x.FullName,
                    Subtitle = x.AdditionalInfo2,
                    Description = x.AboutMe,
                    Price = null,
                    CreatedAt = x.CreatedAt,
                    Distance = x.Distance,
                    LocationLat = x.Location == null ? null : x.Location.Latitude,
                    LocationLng = x.Location == null ? null : x.Location.Longitude,
                    BusinessScore = x.Score
                });
        }

        private IQueryable<SearchResultItem> SearchProducts(SearchRequest searchRequest)
        {
            throw new NotImplementedException("Product search has not yet been refactored into the new format. Use the old implementation instead.");
        }

        private IQueryable<SearchResultItem> SearchPortfolios(SearchRequest searchRequest)
        {
            IQueryable<PortfolioItem> queryable = _portfolioRepository.GetQueryable();
            queryable.Include(x => x.Portfolio);
            queryable.Include(x => x.Portfolio.Items);
            queryable.Include(x => x.Portfolio.Seller);
            queryable.Include(x => x.SubCategories);

            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired;

            var paymentEnabled = MaaSConfig.Current.Payment.Enabled && !MaaSConfig.Current.Search.IgnorePaymentRequirements && !subscriptionEnabled;

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
                x.Price,
                x.SubCategories,
                x.Title,
                x.FriendlyUrl,
                x.VideoUrl,
                x.Portfolio.Seller,
                x.Portfolio.Items,
                x.Portfolio.Seller.Subscription,
                Location = x.Portfolio.Seller.Address != null ? x.Portfolio.Seller.Address.Location : null,
                Distance = dbLocation != null && x.Portfolio.Seller.Address != null && x.Portfolio.Seller.Address.Location != null ? x.Portfolio.Seller.Address.Location.Distance(dbLocation) : null
            })
            .Where(x => x.Items.Any(i => !i.Deleted))
            .Where(x => searchRequest.CategoryId == null || x.SubCategories.Any(s => s.Category_Id == searchRequest.CategoryId))
            .Where(x => searchRequest.SubCategoryId == null || x.SubCategories.Any(s => s.Id == searchRequest.SubCategoryId))
            .Where(x => !useBounds || (x.Location != null && (x.Location.Intersects(dbBounds) || x.Distance <= locationRange)))
            .Where(x => !useLocation || x.Distance <= locationRange)
            .Where(x => searchRequest.CityAndState == null || x.Seller.Address.CityAndState == searchRequest.CityAndState)
            .Where(x => searchRequest.Query == null || searchRequest.Query == "" || x.Title.Contains(searchRequest.Query) || x.Description.Contains(searchRequest.Query))
            .Where(x => !paymentEnabled || (x.Seller.SupplierGatewayId != null && x.Seller.SupplierGatewayId != ""))
            .Where(x => !subscriptionEnabled || (undergoingValidation && x.Seller.Email != validationMail) ||
                   (x.Subscription != null && (
                    x.Subscription.Status == SubscriptionStatus.Trialing ||
                    x.Subscription.Status == SubscriptionStatus.Paid ||
                    x.Subscription.Status == SubscriptionStatus.PendingPayment)))
            .Where(x => x.Seller.Enabled)
            .Where(x => !MaaSConfig.Current.Seller.Moderation.Enabled || x.Seller.RequireModeration != true)
            .Select(x => new SearchResultItem()
            {
                CreatedAt = x.CreatedAt,
                Description = x.Description,
                Title = x.Title,
                FriendlyUrl = x.FriendlyUrl,
                Subtitle = x.Seller.AlternativeName ?? x.Seller.FullName,
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

        private IOrderedQueryable<SearchResultItem> SetSorting(SearchRequest searchRequest, IQueryable<SearchResultItem> items)
        {
            switch (searchRequest.Sorting)
            {
                case SearchSorting.LowerDistance:
                    return items
                        .OrderBy(x => x.Distance)
                        .ThenByDescending(x => x.BusinessScore)
                        .ThenBy(x => x.Title);

                case SearchSorting.Alphabetical:
                    return items
                        .OrderBy(x => x.Title)
                        .ThenByDescending(x => x.BusinessScore);

                case SearchSorting.Newer:
                    return items
                        .OrderByDescending(x => x.CreatedAt)
                        .ThenByDescending(x => x.BusinessScore)
                        .ThenBy(x => x.Title);

                case SearchSorting.MostRelevant:
                case SearchSorting.LowerPrice:
                case SearchSorting.HigherPrice:
                default:
                    return items
                        .OrderBy(x => x.Distance)
                        .ThenByDescending(x => x.BusinessScore)
                        .ThenBy(x => x.Title);
            }
        }
    }
}