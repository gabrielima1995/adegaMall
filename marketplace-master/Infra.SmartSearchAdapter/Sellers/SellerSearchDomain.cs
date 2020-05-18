using SmartSearch;
using SmartSearch.Abstractions;
using _ = Infra.SmartSearchAdapter.Sellers.SellerFieldNames;

namespace Infra.SmartSearchAdapter.Sellers
{
    public class SellerSearchDomain : SearchDomain
    {
        public SellerSearchDomain() : base("Seller", fields)
        {
        }

        private static readonly IField[] fields = new[]
        {
            new Field(_.Id, FieldType.Literal),
            new Field(_.AlternativeName, FieldType.Text, FieldRelevance.Higher, enableSearching: true, enableSorting: true),
            new Field(_.FullName, FieldType.Text, FieldRelevance.High, enableSearching: true, enableSorting: true),
            new Field(_.Nickname, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AboutMe, FieldType.Text, FieldRelevance.Normal, enableSearching: true),
            new Field(_.AdditionalInfo2, FieldType.Text, FieldRelevance.Normal, enableSearching: true),
            new Field(_.Avatar, FieldType.Literal),
            new Field(_.CreatedAt, FieldType.Date),
            new Field(_.Enabled, FieldType.Bool),
            new Field(_.FriendlyUrl, FieldType.Literal),
            new Field(_.SupplierGatewayId, FieldType.Literal),
            new Field(_.CategoryIds, FieldType.IntArray, FieldRelevance.Normal, enableFaceting: true),
            new Field(_.CategoryNames, FieldType.TextArray, FieldRelevance.High, enableFaceting: true, enableSearching: true, enableSorting: true),
            new Field(_.SubCategoryIds, FieldType.IntArray, FieldRelevance.Normal, enableFaceting: true),
            new Field(_.SubCategoryNames, FieldType.TextArray, FieldRelevance.High, enableFaceting: true, enableSearching: true, enableSorting: true),

            new Field(_.SubscriptionStatus, FieldType.Literal),
            new Field(_.SubscriptionStartDate, FieldType.Date),
            new Field(_.SubscriptionFinishDate, FieldType.Date),
            new Field(_.SubscriptionPlanId, FieldType.Literal),
            new Field(_.SubscriptionAmount, FieldType.Double),
            new Field(_.SubscriptionCurrency, FieldType.Literal),
            new Field(_.SubscriptionPostbackUrl, FieldType.Literal),

            new Field(_.AddressId, FieldType.Literal),
            new Field(_.AddressStreet, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressNumber, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressAdditionalInfo, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressNeighborhood, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressCityAndState, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressZipCode, FieldType.Text, FieldRelevance.Normal),
            new Field(_.AddressLatLng, FieldType.LatLng, FieldRelevance.Normal, enableSorting: true),

            new Field(_.AverageRating, FieldType.Double, FieldRelevance.Normal, enableSearching: true, enableSorting: true),
            new Field(_.HasActiveSubscription, FieldType.Bool),
            new Field(_.HasAnyCategories, FieldType.Bool),
            new Field(_.HasAvatar, FieldType.Bool),
            new Field(_.HasPortfolio, FieldType.Double),
            new Field(_.HasServicesOrProducts, FieldType.Double),
            new Field(_.HasBeenApprovedInModeration, FieldType.Bool),
            new Field(_.Score, FieldType.Int, FieldRelevance.Normal, enableSorting: true)
        };
    }
}