using Application.Config;
using Domain.DTO;
using Framework.Geography;
using SmartSearch;
using SmartSearch.Abstractions;
using SmartSearch.LuceneNet;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Cities;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Products;
using V2.Application.Modules.StoreModule.Queries;
using SearchRequest = SmartSearch.SearchRequest;

namespace Infra.SmartSearchAdapter.Products
{
    public class ProductSearchService : IProductSearchService
    {
        private const double MaxLocationRangeInKm = 40000D;

        private readonly ICityQueries cityQueries;

        public ProductSearchService(
            ICityQueries cityQueries)
        {
            this.cityQueries = cityQueries;
        }

        #region Search

        public ProductSearchResult Search(ProductSearchRequest request)
        {
            var internalRequest = new SearchRequest(request.Product ?? request.Query, request.StartIndex, request.PageSize,
                MapFilters(request), MapSortOptions(request));

            var rawResult = new LuceneSearchService(new DefaultIndexOptions())
                .Search(new ProductIndexContext(), new ProductSearchDomain(), internalRequest);

            return SearchResultMapper.MapProductSearchResult(rawResult, request.Bounds?.GetCenter() ?? request.Location);
        }

        public CommonSearchResult Search(CommonSearchRequest request)
        {
            var searchRequest = MapSearchRequest(request);
            var result = SearchInternal(searchRequest);
            return SearchResultMapper.MapProductSearchResult(result);
        }

        private ISearchResult SearchInternal(ISearchRequest request)
        {
            return new LuceneSearchService(new DefaultIndexOptions()).Search(
                new ProductIndexContext(), new ProductSearchDomain(), request);
        }

        private ISearchRequest MapSearchRequest(CommonSearchRequest request)
        {
            var mapped = new SearchRequest(request.Query, request.StartIndex, request.PageSize,
                MapFilters(request),
                MapSortOptions(request));
            return mapped;
        }

        private IFilter[] MapFilters(CommonSearchRequest request)
        {
            var config = MaaSConfig.Current;
            //var onSitePickupShippingEnabled = config.Product.OnSitePickupShipping.Enabled;

            var filters = new List<IFilter>();

            #region Filtros referentes às configurações do Marketplace

            filters.Add(new ValueFilter("HasStock", true));

            if (config.Seller.Moderation.Enabled)
                filters.Add(new ValueFilter("HasBeenApprovedInModeration", true));

            if (config.Product.Templates.Enabled && config.Product.Templates.ForceSellerToUseTemplates)
                filters.Add(new ValueFilter("HasTemplate", true));

            if (config.Subscription.IsRequired && !config.Payment.PagarMe.UndergoingValidation)
                filters.Add(new ValueFilter("HasActiveSubscription", true));

            if (config.Payment.Enabled)
                filters.Add(new ValueFilter("HasPaymentGatewayAccount", true));

            #endregion Filtros referentes às configurações do Marketplace

            filters.Add(new ValueFilter("IsBestPrice", true));

            if (request.CategoryId.HasValue)
                filters.Add(new ValueFilter("CategoryId", request.CategoryId));

            return filters.ToArray();
        }

        private IFilter[] MapFilters(ProductSearchRequest request)
        {
            var config = MaaSConfig.Current;
            var onSitePickupShippingEnabled = config.Product.OnSitePickupShipping.Enabled;

            var filters = new List<IFilter>();

            #region Filtros referentes às configurações do Marketplace

            filters.Add(new ValueFilter("HasStock", true));
            filters.Add(new ValueFilter("IsDisabled", false));

            if (config.Seller.Moderation.Enabled)
                filters.Add(new ValueFilter("HasBeenApprovedInModeration", true));

            if (config.Product.Templates.Enabled && config.Product.Templates.ForceSellerToUseTemplates)
                filters.Add(new ValueFilter("HasTemplate", true));

            if (config.Subscription.IsRequired && !config.Payment.PagarMe.UndergoingValidation)
                filters.Add(new ValueFilter("HasActiveSubscription", true));

            if (config.Payment.Enabled)
                filters.Add(new ValueFilter("HasPaymentGatewayAccount", true));

            #endregion Filtros referentes às configurações do Marketplace

            #region Filtros referentes à entrada do usuário

            if (!string.IsNullOrEmpty(request.SellerId))
            {
                filters.Add(new ValueFilter("SellerId", request.SellerId));
            }
            else
            {
                filters.Add(new ValueFilter("IsBestPrice", true));
            }

            if (request.Category.HasValue)
                filters.Add(new ValueFilter("CategoryId", request.Category.Value));

            if (!string.IsNullOrEmpty(request.CategorySlug))
                filters.Add(new ValueFilter("CategorySlug", request.CategorySlug));

            if (request.VariantIds != null && request.VariantIds.Any())
            {
                filters.AddRange(request.VariantIds
                    .Select(variantId => new ValueFilter("VariantIds", variantId)));
            }

            if (!string.IsNullOrEmpty(request.VariantSlug))
                filters.Add(new ValueFilter("VariantSlugs", request.VariantSlug));

            // apenas produtos com retirada no local
            if (request.OnlyOnsitePickup && onSitePickupShippingEnabled)
                filters.Add(new ValueFilter("OnSitePickupEnabled", true));

            // informada cidade ou localização
            if (!string.IsNullOrEmpty(request.CityAndState) || request.Bounds != null || request.Location != null)
            {
                var latLngFilter = GetLatLngFilterValueOrNull(request);
                if (latLngFilter != null) filters.Add(latLngFilter);
            }

            var hasDateOptions =
                request.OnlySatAndSun || request.OnlyMonToFri ||
                request.OnlyMorning || request.OnlyAfternoon || request.OnlyNight;

            if (hasDateOptions)
            {
                var from = IntDateHelper.Convert(request.StartDate ?? DateTime.Today);
                var to = IntDateHelper.Convert(request.EndDate ?? DateTime.Today.AddDays(365));

                if (request.OnlySatAndSun)
                    filters.Add(new RangeFilter("AvailableOnWeekends", from, to));

                else if (request.OnlyMonToFri)
                    filters.Add(new RangeFilter("AvailableMondayToFriday", from, to));

                if ((request.OnlyMorning || request.OnlyAfternoon || request.OnlyNight) && !(request.OnlyMorning && request.OnlyAfternoon && request.OnlyNight))
                {
                    var periodFilters = new List<IFilter>();

                    if (request.OnlyMorning) periodFilters.Add(new RangeFilter("AvailableMorning", from, to));
                    if (request.OnlyAfternoon) periodFilters.Add(new RangeFilter("AvailableAfternoon", from, to));
                    if (request.OnlyNight) periodFilters.Add(new RangeFilter("AvailableNight", from, to));

                    if (periodFilters.Count == 1) filters.Add(periodFilters[0]);
                    else filters.Add(new FilterGroup(GroupingClause.Or, periodFilters));
                }
            }

            #endregion Filtros referentes à entrada do usuário

            return filters.ToArray();
        }

        private ISortOption[] MapSortOptions(ProductSearchRequest request)
        {
            var sortOptions = new List<ISortOption>();

            switch (request.Sorting)
            {
                case ProductSortType.LowerPrice:
                    sortOptions.Add(new SortOption("BasePrice", SortDirection.Ascending));
                    break;

                case ProductSortType.HigherPrice:
                    sortOptions.Add(new SortOption("BasePrice", SortDirection.Descending));
                    break;

                case ProductSortType.Alphabetical:
                    sortOptions.Add(new SortOption("Title", SortDirection.Ascending));
                    break;

                case ProductSortType.Newer:
                    sortOptions.Add(new SortOption("CreatedAt", SortDirection.Descending));
                    break;

                case ProductSortType.Distance:
                    var distanceSort = GetLatLngSortOptionOrNull(request.CityAndState, request.Bounds, request.Location);
                    if (distanceSort != null) sortOptions.Add(distanceSort);
                    break;

                case ProductSortType.MostRelevant:
                    break;
            }

            sortOptions.Add(new SortOption("AverageRating", SortDirection.Descending));
            sortOptions.Add(new SortOption("SellerCreatedAt", SortDirection.Ascending));
            sortOptions.Add(new SortOption("UpdatedAt", SortDirection.Descending));

            return sortOptions.ToArray();
        }

        private ISortOption[] MapSortOptions(CommonSearchRequest request)
        {
            var sortOptions = new List<ISortOption>();

            switch (request.Sorting)
            {
                case V2.Application.Modules.Search.SearchSorting.LowerPrice:
                    sortOptions.Add(new SortOption("BasePrice", SortDirection.Ascending));
                    break;

                case V2.Application.Modules.Search.SearchSorting.HigherPrice:
                    sortOptions.Add(new SortOption("BasePrice", SortDirection.Descending));
                    break;

                case V2.Application.Modules.Search.SearchSorting.Alphabetical:
                    sortOptions.Add(new SortOption("Title", SortDirection.Ascending));
                    break;

                case V2.Application.Modules.Search.SearchSorting.Newer:
                    sortOptions.Add(new SortOption("CreatedAt", SortDirection.Descending));
                    break;

                case V2.Application.Modules.Search.SearchSorting.LowerDistance:
                    var distanceSort = GetLatLngSortOptionOrNull(null, request.Bounds, request.Location);
                    if (distanceSort != null) sortOptions.Add(distanceSort);
                    break;

                case V2.Application.Modules.Search.SearchSorting.MostRelevant:
                    break;
            }

            sortOptions.Add(new SortOption("AverageRating", SortDirection.Descending));
            sortOptions.Add(new SortOption("SellerCreatedAt", SortDirection.Ascending));
            sortOptions.Add(new SortOption("UpdatedAt", SortDirection.Descending));

            return sortOptions.ToArray();
        }

        private LatLngSortOption GetLatLngSortOptionOrNull(string cityAndState, GeoBounds bounds, GeoCoordinate location)
        {
            LatLng origin = null;
            if (!string.IsNullOrEmpty(cityAndState))
            {
                var latLng = cityQueries.GetCityLatLng(cityAndState);
                if (latLng != null) origin = new LatLng(latLng.Item1, latLng.Item2);
            }
            else if (bounds != null)
            {
                var center = bounds.GetCenter();
                origin = new LatLng(center.Lat, center.Lng);
            }
            else if (location != null)
            {
                origin = new LatLng(location.Lat, location.Lng);
            }

            return origin == null ? null : new LatLngSortOption("AddressLatLng", SortDirection.Ascending, origin);
        }

        private LatLngFilter GetLatLngFilterValueOrNull(ProductSearchRequest request)
        {
            var rangeInKm = (request.LocationRange ?? MaaSConfig.Current.Search.List.SearchRange) / 1000D;

            if (!string.IsNullOrEmpty(request.CityAndState))
            {
                var latLng = cityQueries.GetCityLatLng(request.CityAndState);

                if (latLng != null)
                {
                    return LatLngFilter.CreateRadiusInKm("AddressLatLng",
                        latLng.Item1, latLng.Item2,
                        Math.Min(rangeInKm, MaxLocationRangeInKm));
                }
            }

            if (request.Bounds != null)
            {
                return LatLngFilter.CreateBox("AddressLatLng",
                    new LatLng(request.Bounds.Origin.Lat, request.Bounds.Origin.Lng),
                    new LatLng(request.Bounds.End.Lat, request.Bounds.End.Lng));
            }

            if (request.Location != null)
            {
                return LatLngFilter.CreateRadiusInKm("AddressLatLng",
                    request.Location.Lat, request.Location.Lng,
                    Math.Min(rangeInKm, MaxLocationRangeInKm));
            }

            return null;
        }

        #endregion Search

        #region Nearby

        public ProductSearchResult Nearby(NearbyProductsRequest request)
        {
            var internalRequest = new SearchRequest(request.Query, request.StartIndex, request.PageSize,
                MapFilters(request), MapSortOptions(request));

            var rawResult = new LuceneSearchService(new DefaultIndexOptions())
                .Search(new ProductIndexContext(), new ProductSearchDomain(), internalRequest);

            return SearchResultMapper.MapProductSearchResult(rawResult, request.Location);
        }

        private IFilter[] MapFilters(NearbyProductsRequest request)
        {
            var config = MaaSConfig.Current;

            var filters = new List<IFilter>();

            #region Filtros referentes às configurações do Marketplace

            filters.Add(new ValueFilter("HasStock", true));

            if (config.Seller.Moderation.Enabled)
                filters.Add(new ValueFilter("HasBeenApprovedInModeration", true));

            if (config.Product.Templates.Enabled && config.Product.Templates.ForceSellerToUseTemplates)
                filters.Add(new ValueFilter("HasTemplate", true));

            if (config.Subscription.IsRequired && !config.Payment.PagarMe.UndergoingValidation)
                filters.Add(new ValueFilter("HasActiveSubscription", true));

            if (config.Payment.Enabled)
                filters.Add(new ValueFilter("HasPaymentGatewayAccount", true));

            #endregion Filtros referentes às configurações do Marketplace

            filters.Add(new ValueFilter("IsBestPrice", true));

            #region Filtros referentes à entrada do usuário

            // informada cidade ou localização
            if (request.Bounds != null || request.Location != null)
            {
                var latLngFilter = GetLatLngFilterValueOrNull(request);

                if (latLngFilter != null)
                    filters.Add(latLngFilter);
            }

            #endregion Filtros referentes à entrada do usuário

            return filters.ToArray();
        }

        private ISortOption[] MapSortOptions(NearbyProductsRequest request)
        {
            var sortOptions = new List<ISortOption>();

            if (request.Location != null)
            {
                sortOptions.Add(new LatLngSortOption("AddressLatLng", SortDirection.Ascending, request.Location.Lat, request.Location.Lng));
            }
            else
            {
                sortOptions.Add(new SortOption("AverageRating", SortDirection.Descending));
                sortOptions.Add(new SortOption("SellerCreatedAt", SortDirection.Ascending));
                sortOptions.Add(new SortOption("UpdatedAt", SortDirection.Descending));
            }

            return sortOptions.ToArray();
        }

        private LatLngFilter GetLatLngFilterValueOrNull(NearbyProductsRequest request)
        {
            var rangeInKm = (request.LocationRange ?? MaaSConfig.Current.NearbyProducts.RangeInMeters) / 1000D;

            if (request.Bounds != null)
            {
                return LatLngFilter.CreateBox("AddressLatLng",
                    new LatLng(request.Bounds.Origin.Lat, request.Bounds.Origin.Lng),
                    new LatLng(request.Bounds.End.Lat, request.Bounds.End.Lng));
            }

            if (request.Location != null)
            {
                return LatLngFilter.CreateRadiusInKm("AddressLatLng",
                    request.Location.Lat, request.Location.Lng,
                    Math.Min(rangeInKm, MaxLocationRangeInKm));
            }

            return null;
        }

        #endregion Nearby

        #region Schedule timeline profile

        public List<RentalScheduleTimelineResourceItem> ScheduleTimelineProfile(string sellerId)
        {
            const int pageSize = 500;

            IEnumerable<IFilter> MapFilters()
            {
                var config = MaaSConfig.Current;

                var filters = new List<IFilter>();

                #region Filtros referentes às configurações do Marketplace

                filters.Add(new ValueFilter("HasStock", true));

                if (config.Seller.Moderation.Enabled)
                    filters.Add(new ValueFilter("HasBeenApprovedInModeration", true));

                if (config.Product.Templates.Enabled && config.Product.Templates.ForceSellerToUseTemplates)
                    filters.Add(new ValueFilter("HasTemplate", true));

                if (config.Subscription.IsRequired && !config.Payment.PagarMe.UndergoingValidation)
                    filters.Add(new ValueFilter("HasActiveSubscription", true));

                if (config.Payment.Enabled)
                    filters.Add(new ValueFilter("HasPaymentGatewayAccount", true));

                #endregion Filtros referentes às configurações do Marketplace

                #region Filtros referentes à entrada do usuário

                if (!string.IsNullOrEmpty(sellerId))
                    filters.Add(new ValueFilter("SellerId", sellerId));

                #endregion Filtros referentes à entrada do usuário

                return filters;
            }

            IEnumerable<ISortOption> MapSortOptions()
            {
                var sortOptions = new List<ISortOption>
                {
                    new SortOption("Title", SortDirection.Descending)
                };

                return sortOptions;
            }

            IEnumerable<RentalScheduleTimelineResourceItem> MapResult(ISearchResult rawR)
            {
                foreach (var document in rawR.Documents)
                {
                    yield return RentalScheduleTimelineResourceItem.New(
                        document.Fields["Id"].ToString(),
                        (string)document.Fields["Title"]);
                }
            }

            var internalRequest = new SearchRequest(pageSize: pageSize,
                filters: MapFilters(), sortOptions: MapSortOptions());

            var rawResult = new LuceneSearchService(new DefaultIndexOptions())
                .Search(new ProductIndexContext(), new ProductSearchDomain(), internalRequest);

            return MapResult(rawResult)
                .ToList();
        }

        #endregion Schedule timeline profile
    }
}