using Application.Config;

namespace Domain.Modules.Search
{
    public class SearchSpecs
    {
        public static bool MustCreateOrUpdateIndices(MaaSConfig oldConfig, MaaSConfig newConfig)
        {
            return MustCreateOrUpdateIndicesProduct(oldConfig, newConfig) ||
                MustCreateOrUpdateIndicesQuote(oldConfig, newConfig);
        }

        public static bool MustCreateOrUpdateIndicesProduct(MaaSConfig oldConfig, MaaSConfig newConfig)
        {
            return newConfig.Product.Enabled &&
                   (newConfig.Product.Templates.Enabled && !oldConfig.Product.Templates.Enabled ||
                    newConfig.Product.Templates.ForceSellerToUseTemplates && !oldConfig.Product.Templates.ForceSellerToUseTemplates ||
                    newConfig.Product.ShippingMode != oldConfig.Product.ShippingMode ||
                    newConfig.Subscription.Enabled && !oldConfig.Subscription.Enabled ||
                    newConfig.Seller.Moderation.Enabled && !oldConfig.Seller.Moderation.Enabled ||
                    newConfig.Payment.Enabled && !oldConfig.Payment.Enabled);
        }

        public static bool MustCreateOrUpdateIndicesQuote(MaaSConfig oldConfig, MaaSConfig newConfig)
        {
            return newConfig.Quote.Enabled &&
                   (newConfig.Search.QuotesSearch.Enabled && !oldConfig.Search.QuotesSearch.Enabled);
        }
    }
}