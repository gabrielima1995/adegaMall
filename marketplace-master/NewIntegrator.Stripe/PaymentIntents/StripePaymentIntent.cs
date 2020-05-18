namespace NewIntegrator.Stripe.PaymentIntents
{
    public class StripePaymentIntent
    {
        public string Id { get; internal set; }

        public StripePaymentIntentStatus Status { get; internal set; }

        public string ClientSecret { get; internal set; }

        public long Amount { get; internal set; }
    }
}