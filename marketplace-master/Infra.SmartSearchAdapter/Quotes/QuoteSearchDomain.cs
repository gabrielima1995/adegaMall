using SmartSearch;
using SmartSearch.Abstractions;

namespace Infra.SmartSearchAdapter.Quotes
{
    public class QuoteSearchDomain : SearchDomain
    {
        public QuoteSearchDomain() : base("Quote", fields)
        {
        }

        private static readonly IField[] fields = new[]
        {
            new Field("Id", FieldType.Literal),
            new Field("Title", FieldType.Text, FieldRelevance.Higher, enableSearching: true),
            new Field("Description", FieldType.Text, FieldRelevance.High, enableSearching:true),
            new Field("Phone", FieldType.Literal, FieldRelevance.Normal),
            new Field("Date", FieldType.Date, FieldRelevance.Normal),
            new Field("Preference", FieldType.Int, FieldRelevance.Normal),
            new Field("Amount", FieldType.Double, FieldRelevance.Normal),
            new Field("Category", FieldType.Int, FieldRelevance.Normal),
            new Field("SubCategory", FieldType.Int, FieldRelevance.Normal),
            new Field("CreatedAt", FieldType.Date, FieldRelevance.Normal, enableSorting: true),
            new Field("GroupId", FieldType.Literal, FieldRelevance.Normal),
            new Field("BuyerId", FieldType.Literal, FieldRelevance.Normal),
            new Field("BuyerName", FieldType.Text, FieldRelevance.Normal),
            new Field("BuyerEmail", FieldType.Literal, FieldRelevance.Normal),
            new Field("CategoryName", FieldType.Literal, FieldRelevance.Normal, enableSearching: true),
            new Field("SubCategoryName", FieldType.Literal, FieldRelevance.Normal, enableSearching: true),
            new Field("PortfolioItem", FieldType.Literal, FieldRelevance.Normal),
            new Field("PortfolioItemTitle", FieldType.Literal, FieldRelevance.Normal, enableSearching: true),
            new Field("Quantity", FieldType.Literal, FieldRelevance.Normal),
            new Field("CityAndState", FieldType.Text, FieldRelevance.Normal, enableSearching: true)
        };
    }
}