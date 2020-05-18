using Application.Config;
using SmartSearch;
using SmartSearch.Abstractions;
using SmartSearch.LuceneNet;
using System;
using System.Collections.Generic;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Sellers;

namespace Infra.SmartSearchAdapter.Sellers
{
    public class SellerSearchService : ISellerSearchService
    {
        public CommonSearchResult Search(CommonSearchRequest request)
        {
            var searchRequest = MapSearchRequest(request);
            var result = SearchInternal(searchRequest);
            return SearchResultMapper.MapSellerSearchResult(result);
        }

        private ISearchResult SearchInternal(ISearchRequest request)
        {
            return new LuceneSearchService(new DefaultIndexOptions()).Search(
                new SellerIndexContext(), new SellerSearchDomain(), request);
        }

        private ISearchRequest MapSearchRequest(CommonSearchRequest request)
        {
            var mapped = new SearchRequest(request.Query, request.StartIndex, request.PageSize, MapFilters(request), MapSortOptions(request));
            return mapped;
        }

        private IFilter[] MapFilters(CommonSearchRequest request)
        {
            var filters = new List<IFilter>();

            if (request.CategoryId.HasValue)
                filters.Add(new ValueFilter(SellerFieldNames.CategoryIds, request.CategoryId.Value));

            if (request.SubCategoryId.HasValue)
                filters.Add(new ValueFilter(SellerFieldNames.SubCategoryIds, request.SubCategoryId.Value));

            if(MaaSConfig.Current.Seller.Moderation.Enabled)
                filters.Add(new ValueFilter(SellerFieldNames.HasBeenApprovedInModeration, true));

            if (request.Bounds != null || request.Location != null)
            {
                var latLngFilter = GetLatLngFilterValueOrNull(request);
                if (latLngFilter != null) filters.Add(latLngFilter);
            }

            if (request.FilterBestResultsForHome)
            {
                if (MaaSConfig.Current.Product.Enabled && !MaaSConfig.Current.Quote.QuoteBilling.Enabled)
                    filters.Add(new ValueFilter(SellerFieldNames.HasServicesOrProducts, true));

                if (MaaSConfig.Current.Quote.Enabled)
                {
                    filters.Add(new ValueFilter(SellerFieldNames.HasAnyCategories, true));
                    filters.Add(new ValueFilter(SellerFieldNames.HasPortfolio, true));
                }
            }

            return filters.ToArray();
        }

        private SortOption[] MapSortOptions(CommonSearchRequest request)
        {
            var sortOptions = new List<SortOption>();

            switch (request.Sorting)
            {
                case SearchSorting.Alphabetical:
                    sortOptions.Add(new SortOption(SellerFieldNames.AlternativeName, SortDirection.Ascending));
                    sortOptions.Add(new SortOption(SellerFieldNames.FullName, SortDirection.Ascending));
                    break;

                case SearchSorting.LowerDistance:
                    if (request.Location != null)
                        sortOptions.Add(new LatLngSortOption(SellerFieldNames.AddressLatLng, SortDirection.Ascending, request.Location.Lat, request.Location.Lng));
                    break;

                case SearchSorting.MostRelevant:
                case SearchSorting.LowerPrice:
                case SearchSorting.HigherPrice:
                case SearchSorting.Newer:
                default: break;
            }

            sortOptions.Add(new SortOption(SellerFieldNames.Score, SortDirection.Descending));

            return sortOptions.ToArray();
        }

        private LatLngFilter GetLatLngFilterValueOrNull(CommonSearchRequest request)
        {
            if (request.Location != null && MaaSConfig.Current.Search.Map.Enabled)
            {
                var rangeInKm = (request.LocationRange ?? MaaSConfig.Current.Search.Map.SearchRange) / 1000;

                return LatLngFilter.CreateRadiusInKm(SellerFieldNames.AddressLatLng,
                    request.Location.Lat, request.Location.Lng,
                    Math.Min(rangeInKm, 40000));
            }

            if (request.Bounds != null)
            {
                return LatLngFilter.CreateBox(SellerFieldNames.AddressLatLng,
                    new LatLng(request.Bounds.Origin.Lat, request.Bounds.Origin.Lng),
                    new LatLng(request.Bounds.End.Lat, request.Bounds.End.Lng));
            }

            if (request.Location != null)
            {
                var rangeInKm = (request.LocationRange ?? MaaSConfig.Current.Search.Map.SearchRange) / 1000;

                return LatLngFilter.CreateRadiusInKm(SellerFieldNames.AddressLatLng,
                    new LatLng(request.Location.Lat, request.Location.Lng),
                    Math.Min(rangeInKm, 40000));
            }

            return null;
        }
    }
}