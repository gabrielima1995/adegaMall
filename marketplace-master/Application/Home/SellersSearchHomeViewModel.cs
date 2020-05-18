using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;
using System.Globalization;

namespace Application.Home
{
    public class SellersSearchHomeViewModel : IWithCategory, IWithSubCategory, IWithCity
    {
        public int? Category { get; set; }

        public int? SubCategory { get; set; }

        public string Seller { get; set; }

        public string CityAndState { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public string Lang { get; set; }

        public IList<Category> Categories { get; set; } = new List<Category>();

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public SellersSearchHomeViewModel()
        {
            Lang = CultureInfo.CurrentCulture.GetCurrentLang();
        }

        public SellersSearchHomeViewModel(IList<Category> categories, IList<SubCategory> subCategories) : this()
        {
            Categories = categories;
            SubCategories = subCategories;
        }
    }
}