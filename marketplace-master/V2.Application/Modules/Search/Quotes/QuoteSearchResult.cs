using Domain.Entities.PortfolioModule;
using System;

namespace V2.Application.Modules.Search.Quotes
{
    public class QuoteSearchResult : SearchResultBase<QuoteSearchResultItem>
    {
    }

    public class QuoteSearchResultItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Phone { get; set; }

        public DateTime? Date { get; set; }

        public QuotePreference Preference { get; set; }

        public decimal? Amount { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid GroupId { get; set; }

        public string CityAndState { get; set; }

        public string BuyerName { get; set; }

        public string BuyerEmail { get; set; }

        public Guid BuyerId { get; set; }

        public bool IsShortQuote { get; set; }

        public Guid? PortfolioItemId { get; set; }

        public string PortfolioItemTitle { get; set; }

        public int? Quantity { get; set; }
    }
}