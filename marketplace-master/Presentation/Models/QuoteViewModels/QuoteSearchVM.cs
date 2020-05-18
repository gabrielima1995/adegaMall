using Application.Config;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using V2.Application.Modules.Search.Quotes;

namespace Presentation.Models.QuoteViewModels
{
    public class QuoteSearchVM : IWithCategory, IWithSubCategory
    {
        public string Query { get; set; }

        [DisplayNameLocalized("QUOTE_SEARCH_MIN_PRICE")]
        public decimal? MinPrice { get; set; }

        [DisplayNameLocalized("QUOTE_SEARCH_MAX_PRICE")]
        public decimal? MaxPrice { get; set; }

        [DisplayNameLocalized("QUOTE_SEARCH_MIN_DATE")]
        public DateTime? MinDate { get; set; }

        [DisplayNameLocalized("QUOTE_SEARCH_MAX_DATE")]
        public DateTime? MaxDate { get; set; }

        //Geolocation
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string BoundsAX { get; set; }
        public string BoundsAY { get; set; }
        public string BoundsBX { get; set; }
        public string BoundsBY { get; set; }

        public int? Category { get; set; }
        public int? SubCategory { get; set; }
        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public IList<Category> Categories { get; set; } = new List<Category>();

        public QuoteSort OrderBy { get; set; }

        public int Page { get; set; }

        public QuoteSearchVM()
        {

        }

        public QuoteSearchRequest ToSearchRequest()
        {
            return new QuoteSearchRequest()
            {
                PageSize = MaaSConfig.Current.Search.QuotesSearch.ResultsPerPage,
                Query = Query,
                StartIndex = Math.Max(0, Page * MaaSConfig.Current.Search.QuotesSearch.ResultsPerPage),
                Sort = OrderBy,
                MinDate = MinDate,
                MaxDate = MaxDate,
                MinPrice = MinPrice.HasValue ? Convert.ToDouble(MinPrice.Value) : (double?)null,
                MaxPrice = MaxPrice.HasValue ? Convert.ToDouble(MaxPrice.Value) : (double?)null,
                Category = Category,
                SubCategory = SubCategory,
            };
        }
    }

    //public enum OrderingType
    //{
    //    [Display(Name = "Relevância")]
    //    Relevance,

    //    [Display(Name = "Mais recente")]
    //    MostRecent,

    //    [Display(Name = "Mais antigo")]
    //    LeastRecent,
    //}
}