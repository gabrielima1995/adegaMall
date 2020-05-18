using SmartSearch.Abstractions;
using System;
using V2.Application.Modules.Search;

namespace Infra.SmartSearchAdapter.Sellers
{
    public class SearchResultMapper
    {
        public static CommonSearchResult MapSellerSearchResult(ISearchResult rawResult)
        {
            var result = new CommonSearchResult
            {
                Items = new CommonSearchResultItem[rawResult.Documents.Length],
                TotalCount = rawResult.TotalCount
            };

            for (int i = 0; i < rawResult.Documents.Length; i++)
                result.Items[i] = MapDocumentToQuoteSearchResultItem(rawResult.Documents[i]);

            return result;
        }

        private static CommonSearchResultItem MapDocumentToQuoteSearchResultItem(IDocument document)
        {
            var latlng = (ILatLng)document.Fields[SellerFieldNames.AddressLatLng];

            var result = new CommonSearchResultItem
            {
                Id = (string)document.Fields[SellerFieldNames.Id],
                Handle = (string)document.Fields[SellerFieldNames.FriendlyUrl],
                Title = (string)document.Fields[SellerFieldNames.AlternativeName] ?? (string)document.Fields[SellerFieldNames.FullName],
                Subtitle = null,
                Description = (string)document.Fields[SellerFieldNames.AboutMe],
                ImageUrl = (string)document.Fields[SellerFieldNames.Avatar],
                Price = null,
                CreatedAt = (DateTime)document.Fields[SellerFieldNames.CreatedAt],
                Distance = null,
                LocationLat = latlng?.Latitude,
                LocationLng = latlng?.Longitude,
                BusinessScore = Convert.ToInt32(document.Fields[SellerFieldNames.Score])
            };

            return result;
        }
    }
}