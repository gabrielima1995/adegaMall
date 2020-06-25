using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Application.Modules.Portfolio.Search
{
    public class SearchViewModel : SearchResultViewModel, IWithCategory, IWithSubCategory, IWithLang, IWithCity
    {
        public int? Category { get; set; }

        public int? SubCategory { get; set; }

        public string Seller { get; set; }

        public string CityAndState { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public string AdditionalInfo2 { get; set; }

        public bool OnlyAvatar { get; set; }

        public string Lang { get; set; }

        public IList<Category> Categories { get; set; }

        public IList<SubCategory> SubCategories { get; set; }

        public int Total { get; set; }

        public SearchViewModel(SearchRequestViewModel request)
        {
            Category = request.SubCategory;
            SubCategory = request.SubCategory;
            Seller = request.Seller;
            CityAndState = request.CityAndState;
            Lat = request.Lat;
            Lng = request.Lng;
            AdditionalInfo2 = request.AdditionalInfo2;
            OnlyAvatar = request.OnlyAvatar;
            Lang = request.Lang;
        }
    }
}