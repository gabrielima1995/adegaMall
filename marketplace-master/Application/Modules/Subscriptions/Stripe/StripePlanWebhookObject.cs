namespace Application.Modules.Subscriptions.Stripe
{
    public class StripePlanWebhookObject
    {
        public string id { get; set; }

        public long amount { get; set; }

        public string currency { get; set; }

        public string nickname { get; set; }

        public string product { get; set; }

        public string interval { get; set; }

        public long interval_count { get; set; }
    }
}