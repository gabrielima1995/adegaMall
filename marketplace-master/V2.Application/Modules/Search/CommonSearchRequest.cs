using Framework.Geography;
using Infra.Crosscutting.Globalization.Attributes;

namespace V2.Application.Modules.Search
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

    public class CommonSearchRequest : SearchRequestBase
    {
        public GeoBounds Bounds { get; set; }
        public int? CategoryId { get; set; }
        public GeoCoordinate Location { get; set; }
        public long? LocationRange { get; set; }
        public SearchSorting Sorting { get; set; }
        public int? SubCategoryId { get; set; }
        public SearchRequestType Type { get; set; }
        public bool FilterBestResultsForHome { get; set; }
    }
}