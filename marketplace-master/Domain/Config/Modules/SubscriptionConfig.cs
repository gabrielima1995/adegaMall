using Newtonsoft.Json;

namespace Application.Config.Modules
{
    public class SubscriptionConfig
    {
        public bool Enabled { get; set; } = false;

        public bool Mandatory { get; set; } = true;

        public SubscriptionStripeConfig Stripe { get; set; } = new SubscriptionStripeConfig();

        [JsonIgnore]
        public bool IsRequired => Enabled && Mandatory;
    }

    public class SubscriptionStripeConfig
    {
        public string ProductId { get; set; }
    }
}