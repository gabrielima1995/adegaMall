using Domain.Entities.PortfolioModule;
using SmartSearch.Abstractions;
using System;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Quotes;

namespace Infra.SmartSearchAdapter.Quotes
{
    public static class SearchResultMapper
    {
        public static QuoteSearchResult MapQuoteSearchResult(ISearchResult rawResult)
        {
            var result = new QuoteSearchResult
            {
                Items = new QuoteSearchResultItem[rawResult.Documents.Length],
                TotalCount = rawResult.TotalCount
            };

            for (int i = 0; i < rawResult.Documents.Length; i++)
                result.Items[i] = MapDocumentToQuoteSearchResultItem(rawResult.Documents[i]);

            return result;
        }

        private static QuoteSearchResultItem MapDocumentToQuoteSearchResultItem(IDocument document)
        {
            var result = new QuoteSearchResultItem
            {
                Amount = Convert.ToDecimal(document.Fields["Amount"]),
                BuyerEmail = (string)document.Fields["BuyerEmail"],
                BuyerId = new Guid((string)document.Fields["BuyerId"]),
                BuyerName = (string)document.Fields["BuyerName"],
                CategoryId = document.Fields["Category"] == null ? default(int?) : Convert.ToInt32(document.Fields["Category"]),
                CategoryName = (string)document.Fields["CategoryName"],
                CityAndState = (string)document.Fields["CityAndState"],
                CreatedAt = (DateTime)document.Fields["CreatedAt"],
                Date = (DateTime?)document.Fields["Date"],
                Description = (string)document.Fields["Description"],
                GroupId = new Guid((string)document.Fields["GroupId"]),
                Phone = (string)document.Fields["Phone"],
                Preference = (QuotePreference)Convert.ToInt32(document.Fields["Preference"]),
                SubCategoryId = document.Fields["SubCategory"] == null ? default(int?) : Convert.ToInt32(document.Fields["SubCategory"]),
                SubCategoryName = (string)document.Fields["SubCategoryName"],
                Title = (string)document.Fields["Title"],
                PortfolioItemId = document.Fields["PortfolioItem"] == null ? default(Guid?) : new Guid((string)document.Fields["PortfolioItem"]),
                PortfolioItemTitle = (string)document.Fields["PortfolioItemTitle"],
                Quantity = document.Fields["Quantity"] == null ? default(int?) : Convert.ToInt32(document.Fields["SubCategory"]),
            };

            return result;
        }


        public static CommonSearchResult MapProductSearchResultSearchPage(ISearchResult rawResult)
        {
            var result = new CommonSearchResult
            {
                Items = new CommonSearchResultItem[rawResult.Documents.Length],
                TotalCount = rawResult.TotalCount,
            };

            for (var i = 0; i < rawResult.Documents.Length; i++)
            {
                result.Items[i] = MapDocumentToProductSearchResultItem(rawResult.Documents[i]);
            }

            return result;
        }

        private static CommonSearchResultItem MapDocumentToProductSearchResultItem(IDocument document)
        {
            var result = new CommonSearchResultItem
            {
                Id = document.Fields["Id"].ToString(),
                Title = (string)document.Fields["Title"],
                Description = (string)document.Fields["Description"],
                ImageUrl = (string)document.Fields["CoverImageUrl"],
                Handle = (string)document.Fields["FriendlyUrl"]
            };

            return result;
        }
    }
}