using Domain.Modules.StoreModule.Price;
using Framework.Geography;
using Newtonsoft.Json;
using SmartSearch.Abstractions;
using System;
using System.Linq;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Products;

namespace Infra.SmartSearchAdapter.Products
{
    public static class SearchResultMapper
    {
        public static ProductSearchResult MapProductSearchResult(ISearchResult rawResult, GeoCoordinate location = null)
        {
            var result = new ProductSearchResult
            {
                Items = new ProductSearchResultItem[rawResult.Documents.Length],
                TotalCount = rawResult.TotalCount,
                Facets = rawResult.Facets
                    .Where(i => i.Name == "VariantIds")
                    .Select(i => new ProductSearchResult.FacetLabel(i.Label, i.Count))
                    .ToList()
            };

            for (var i = 0; i < rawResult.Documents.Length; i++)
            {
                result.Items[i] = MapDocumentToProductSearchResultItem(rawResult.Documents[i], location);
            }

            return result;
        }

        private static ProductSearchResultItem MapDocumentToProductSearchResultItem(IDocument document, GeoCoordinate location)
        {
            var productPriceStr = (string)document.Fields["ProductPrice"];

            var result = new ProductSearchResultItem
            {
                Id = Guid.Parse(document.Fields["Id"].ToString()),
                Title = (string)document.Fields["Title"],
                Description = (string)document.Fields["Description"],
                CoverImageUrl = (string)document.Fields["CoverImageUrl"],
                FriendlyUrl = (string)document.Fields["FriendlyUrl"],
                Price = productPriceStr == null ? null : JsonConvert.DeserializeObject<ProductPrice>(productPriceStr),
                Images = (document.Fields["ImageUrls"] as object[])?.Cast<string>().ToArray(),
                SellerProfileName = (string)document.Fields["SellerProfileName"],
                CategoryId = document.Fields["CategoryId"] == null ? default(int?) : Convert.ToInt32(document.Fields["CategoryId"]),
                CategoryName = (string)document.Fields["CategoryName"],
                AddressCityAndState = (string)document.Fields["AddressCityAndState"],
                VariantIds = (document.Fields["VariantIds"] as object[])?.Cast<long>().ToArray(),
                VariantSlugs = (document.Fields["VariantSlugs"] as object[])?.Cast<string>().ToArray(),
                Distance = GetDistance(document, location)
            };

            return result;
        }

        public static CommonSearchResult MapProductSearchResult(ISearchResult rawResult)
        {
            var result = new CommonSearchResult
            {
                Items = new CommonSearchResultItem[rawResult.Documents.Length],
                TotalCount = rawResult.TotalCount,
            };

            for (var i = 0; i < rawResult.Documents.Length; i++)
                result.Items[i] = MapDocumentToProductSearchResultItem(rawResult.Documents[i]);

            return result;
        }

        private static CommonSearchResultItem MapDocumentToProductSearchResultItem(IDocument document)
        {
            var productPriceStr = (string)document.Fields["ProductPrice"];

            var result = new CommonSearchResultItem
            {
                Id = document.Fields["Id"].ToString(),
                Title = (string)document.Fields["Title"],
                Description = (string)document.Fields["Description"],
                ImageUrl = (string)document.Fields["CoverImageUrl"],
                Handle = (string)document.Fields["FriendlyUrl"],
                Price = productPriceStr == null ? 0M : JsonConvert.DeserializeObject<ProductPrice>(productPriceStr).DisplayShortPrice.Value
            };

            return result;
        }

        private static string GetDistance(IDocument document, GeoCoordinate location)
        {
            var distance = string.Empty;

            if (location != null)
            {
                var latlng = (ILatLng)document.Fields["AddressLatLng"];
                var target = new GeoCoordinate(latlng.Latitude, latlng.Longitude);
                var distanceInKm = location.DistanceTo(target, UnitOfLength.Kilometers);

                distance = distanceInKm >= 1
                    ? $"{((decimal)distanceInKm).DisplayDecimal(1)} km"
                    : $"{(int)Math.Round(distanceInKm * 1000)} metros";
            }

            return distance;
        }
    }
}