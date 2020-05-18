using Stripe;

namespace NewIntegrator.Stripe
{
    public static class StripeSetup
    {
        public static void Init(string secretKey, string publishableKey)
        {
            StripeConfiguration.ApiKey = secretKey;
            StripeConfiguration.ClientId = publishableKey;
            StripeConfiguration.AppInfo = new AppInfo
            {
                Name = "INA Marketplace v2"
            };
        }
    }
}