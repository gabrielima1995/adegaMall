using Application.Config;
using Application.Search;
using System.Collections.Specialized;

namespace Application.Modules.Portfolio.Search
{
    public class SearchRequestViewModel : SearchRequest
    {
        public int? Category { get; set; }
        public int? SubCategory { get; set; }
        public string Seller { get; set; }
        public string CityAndState { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string AdditionalInfo2 { get; set; }
        public bool OnlyAvatar { get; set; }

        protected override int GetLimitDefault()
        {
            return MaaSConfig.Current.Search.List.PageSize;
        }

        public override string GetNextUrlParams()
        {
            var values = new NameValueCollection
            {
                { "Category", Category },
                { "SubCategory", SubCategory },
                { "Seller", Seller },
                { "CityAndState", CityAndState },
                { "Lat", Lat },
                { "Lng", Lng },
                { "AdditionalInfo2", AdditionalInfo2 },
                { "OnlyAvatar", OnlyAvatar },
                { "Limit", Limit },
                { "Page", Page + 1 },
                { "Area", "" }
            };

            return values.ToQueryString();
        }
    }
}