namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeSubscriptionPostbackData
    {
        public string id { get; set; }

        public string status { get; set; }

        public long created { get; set; }
    }
}