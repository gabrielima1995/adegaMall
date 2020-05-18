using SmartSearch;
using SmartSearch.Abstractions;
using SmartSearch.LuceneNet;
using System;
using System.Collections.Generic;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Quotes;

namespace Infra.SmartSearchAdapter.Quotes
{
    public class QuoteSearchService : IQuoteSearchService
    {
        public QuoteSearchResult Search(QuoteSearchRequest request)
        {
            var rawResult = SearchInternal(request);
            return MapResults(rawResult);
        }

        private QuoteSearchResult MapResults(ISearchResult rawResult)
        {
            return SearchResultMapper.MapQuoteSearchResult(rawResult);
        }

        private ISearchResult SearchInternal(QuoteSearchRequest request)
        {
            var internalRequest = MapSearchRequest(request);

            return new LuceneSearchService(new DefaultIndexOptions())
                .Search(new QuoteIndexContext(), new QuoteSearchDomain(), internalRequest);
        }

        private ISearchRequest MapSearchRequest(QuoteSearchRequest request)
        {
            var builder = new SearchRequestBuilder(request.Query, request.StartIndex, request.PageSize);

            switch (request.Sort)
            {
                case QuoteSort.MostRecent:
                    builder.SortBy("CreatedAt", SortDirection.Descending);
                    break;

                case QuoteSort.Oldest:
                    builder.SortBy("CreatedAt", SortDirection.Ascending);
                    break;
            }

            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
                builder.FilterBy("Amount", request.MinPrice.GetValueOrDefault(), request.MaxPrice.GetValueOrDefault(int.MaxValue));

            if (request.MinDate.HasValue || request.MaxDate.HasValue)
                builder.FilterBy("CreatedAt", request.MinDate.GetValueOrDefault(DateTime.MinValue), request.MaxDate.GetValueOrDefault(DateTime.MaxValue));

            if (request.Category.HasValue)
                builder.FilterBy("Category", request.Category.Value);

            if (request.SubCategory.HasValue)
                builder.FilterBy("SubCategory", request.SubCategory.Value);

            return builder.Build();
        }

        public CommonSearchResult Search(CommonSearchRequest request)
        {
            var searchRequest = MapSearchRequest(request);
            var result = SearchInternal(searchRequest);
            return SearchResultMapper.MapProductSearchResultSearchPage(result);
        }

        private ISearchResult SearchInternal(ISearchRequest request)
        {
            return new LuceneSearchService(new DefaultIndexOptions()).Search(
                new QuoteIndexContext(), new QuoteSearchDomain(), request);
        }

        private ISearchRequest MapSearchRequest(CommonSearchRequest request)
        {
            var mapped = new SearchRequest(request.Query, request.StartIndex, request.PageSize, MapFilters(request), MapSortOptions(request));
            return mapped;
        }

        private List<IFilter> MapFilters(CommonSearchRequest request)
        {
            var filters = new List<IFilter>();

            if (request.CategoryId.HasValue)
                filters.Add(new ValueFilter("Category", request.CategoryId.Value));

            if (request.SubCategoryId.HasValue)
                filters.Add(new ValueFilter("SubCategory", request.SubCategoryId.Value));

            return filters;
        }

        private List<ISortOption> MapSortOptions(CommonSearchRequest request)
        {
            var sortOptions = new List<ISortOption>();

            switch (request.Sorting)
            {
                case SearchSorting.Alphabetical:
                    sortOptions.Add(new SortOption("Title", SortDirection.Ascending));
                    break;

                case SearchSorting.Newer:
                    sortOptions.Add(new SortOption("CreatedAt", SortDirection.Descending));
                    break;

                case SearchSorting.MostRelevant:
                case SearchSorting.LowerPrice:
                case SearchSorting.HigherPrice:
                default: break;
            }

            return sortOptions;
        }
    }
}