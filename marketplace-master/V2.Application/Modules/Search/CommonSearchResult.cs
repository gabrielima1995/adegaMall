using System;

namespace V2.Application.Modules.Search
{
    public class CommonSearchResult : SearchResultBase<CommonSearchResultItem>
    {
    }

    public class CommonSearchResultItem
    {
        public string Id { get; set; }

        public string Handle { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal? Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public double? Distance { get; set; }

        public double? LocationLat { get; set; }

        public double? LocationLng { get; set; }

        public decimal BusinessScore { get; set; }
    }
}