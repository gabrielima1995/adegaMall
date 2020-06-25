namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeInvoiceWebhookObject
    {
        public string id { get; set; }

        public string number { get; set; }

        public string status { get; set; }

        public bool paid { get; set; }

        public string currency { get; set; }

        public string customer { get; set; }

        public string payment_intent { get; set; }

        public string subscription { get; set; }

        public long total { get; set; }
    }
}