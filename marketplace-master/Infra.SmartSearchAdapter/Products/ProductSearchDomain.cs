using SmartSearch;
using SmartSearch.Abstractions;

namespace Infra.SmartSearchAdapter.Products
{
    public class ProductSearchDomain : SearchDomain
    {
        public ProductSearchDomain() : base("ProductV20200401", fields)
        {
        }

        private static readonly IField[] fields = new[]
        {
            new Field("Id", FieldType.Literal),
            new Field("Title", FieldType.Text, FieldRelevance.High, enableSearching: true, enableSorting: true),
            new Field("Description", FieldType.Text, FieldRelevance.High, enableSearching: true),
            new Field("Specifications", FieldType.Text, FieldRelevance.High, enableSearching: true),
            new Field("CustomId", FieldType.Text, FieldRelevance.High, enableSearching: true),
            new Field("CoverImageUrl", FieldType.Literal),
            new Field("FriendlyUrl", FieldType.Literal),
            new Field("CreatedAt", FieldType.Date, FieldRelevance.Normal, enableSorting: true),
            new Field("UpdatedAt", FieldType.Date, FieldRelevance.Normal, enableSorting: true),
            new Field("ProductPrice", FieldType.Literal),
            new Field("BasePrice", FieldType.Double, FieldRelevance.Normal, enableSorting: true),

            new Field("HasStock", FieldType.Bool),
            new Field("IsDisabled", FieldType.Bool),
            new Field("HasTemplate", FieldType.Bool),
            new Field("NoShippingEnabled", FieldType.Bool),
            new Field("ActualNoShippingEnabled", FieldType.Bool),
            new Field("OnSitePickupEnabled", FieldType.Bool),
            new Field("SedexEnabled", FieldType.Bool),
            new Field("PACEnabled", FieldType.Bool),
            new Field("PrivateFleetEnabled", FieldType.Bool),

            new Field("CategoryId", FieldType.Int, FieldRelevance.Normal, enableSearching: true),
            new Field("CategoryName", FieldType.Text, FieldRelevance.High, enableSearching: true),
            new Field("CategorySlug", FieldType.Text, FieldRelevance.Normal, enableSearching: true),

            new Field("SellerId", FieldType.Literal),
            new Field("SellerNickname", FieldType.Literal),
            new Field("SellerProfileName", FieldType.Text, FieldRelevance.High, enableSearching: true),
            new Field("SellerFriendlyUrl", FieldType.Literal),
            new Field("SellerCreatedAt", FieldType.Date, FieldRelevance.Normal, enableSorting: true),

            new Field("ImageUrls", FieldType.LiteralArray, FieldRelevance.Normal),
            new Field("DeliveryRanges", FieldType.IntArray, FieldRelevance.Normal, enableSearching: true),
            new Field("VariantIds", FieldType.IntArray, FieldRelevance.Normal, enableFaceting: true, enableSearching: true),
            new Field("VariantNames", FieldType.LiteralArray, FieldRelevance.Normal, enableFaceting: true, enableSearching: true),
            new Field("VariantSlugs", FieldType.LiteralArray, FieldRelevance.Normal, enableFaceting: true, enableSearching: true),

            new Field("AddressCityAndState", FieldType.Text),
            new Field("AddressLatLng", FieldType.LatLng, FieldRelevance.Normal, enableSorting: true),

            new Field("HasActiveSubscription", FieldType.Bool),
            new Field("HasPaymentGatewayAccount", FieldType.Bool),
            new Field("HasBeenApprovedInModeration", FieldType.Bool),
            new Field("AverageRating", FieldType.Double, FieldRelevance.Normal, enableSorting: true),

            new Field("AllPeriods", FieldType.IntArray),
            new Field("AvailableOnWeekends", FieldType.IntArray),
            new Field("AvailableMondayToFriday", FieldType.IntArray),
            new Field("AvailableMorning", FieldType.IntArray),
            new Field("AvailableAfternoon", FieldType.IntArray),
            new Field("AvailableNight", FieldType.IntArray)
        };
    }
}