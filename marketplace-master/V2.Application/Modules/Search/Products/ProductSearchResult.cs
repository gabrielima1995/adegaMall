using Domain.Modules.StoreModule.Interfaces;
using Domain.Modules.StoreModule.Price;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.Search.Products
{
    public class ProductSearchResult : SearchResultBase<ProductSearchResultItem>
    {
        public IList<FacetLabel> Facets { get; set; }

        public class FacetLabel
        {
            public string Label { get; set; }
            public int Count { get; set; }

            public FacetLabel(string label, int count)
            {
                Label = label;
                Count = count;
            }
        }
    }

    public class ProductSearchResultItem : IWishlistItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CoverImageUrl { get; set; }

        public string FriendlyUrl { get; set; }

        public ProductPrice Price { get; set; }

        public IList<string> Images { get; set; }

        public string SellerProfileName { get; set; }

        public string Distance { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategorySlug { get; set; }

        public string AddressCityAndState { get; set; }

        public long[] VariantIds { get; set; }
    }
}