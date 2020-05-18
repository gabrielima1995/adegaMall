using Framework.Geography;
using Infra.Crosscutting.Globalization.Attributes;
using System;
using System.Collections.Generic;

namespace Domain.DTO
{
    public enum SearchRequestType
    {
        Seller,
        Product,
        Portfolio,
    }

    public enum SearchSorting
    {
        [DescriptionLocalized("SEARCH_SORT_MOST_RELEVANT")]
        MostRelevant = 0,

        [DescriptionLocalized("SEARCH_SORT_LOWER_DISTANCE")]
        LowerDistance,

        [DescriptionLocalized("SEARCH_SORT_LOWER_PRICE")]
        LowerPrice,

        [DescriptionLocalized("SEARCH_SORT_HIGHER_PRICE")]
        HigherPrice,

        [DescriptionLocalized("SEARCH_SORT_ALPHABETICAL")]
        Alphabetical,

        [DescriptionLocalized("SEARCH_SORT_NEWER")]
        Newer
    }

    public class SearchRequest
    {
        public SearchRequestType Type { get; set; }
        public SearchSorting Sorting { get; set; }

        public string Query { get; set; }
        public string CityAndState { get; set; }

        public GeoCoordinate Location { get; set; }
        public GeoBounds Bounds { get; set; }
        public long? LocationRange { get; set; }

        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool OnlyWithPicture { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchResults
    {
        public List<SearchResultItem> Items { get; set; }
        public int TotalItems { get; set; }
        public SearchRequest Request { get; set; }
    }

    public class SearchResultItem
    {
        public string Id { get; set; }
        public string Handle { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FriendlyUrl { get; set; }

        public double? Distance { get; set; }
        public double? LocationLat { get; set; }
        public double? LocationLng { get; set; }

        public decimal BusinessScore { get; set; }
    }
}
