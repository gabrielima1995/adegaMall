namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeSubscriptionWebhookObject
    {
        public string id { get; set; }

        public string status { get; set; }

        public StripePlanWebhookObject plan { get; set; }

        public long created { get; set; }

        public string latest_invoice { get; set; }

        public long? current_period_end { get; set; }

        public long? current_period_start { get; set; }

        public string customer { get; set; }

        public long? start { get; set; }

        public long? ended_at { get; set; }

        public long? canceled_at { get; set; }
    }
}