using NewIntegrator.Stripe.PaymentIntents;

namespace NewIntegrator.Stripe.Invoices
{
    public class StripeInvoice
    {
        public string Id { get; internal set; }

        public string Number { get; internal set; }

        public bool Paid { get; internal set; }

        public StripePaymentIntent PaymentIntent { get; internal set; }

        public StripeInvoiceStatus Status { get; internal set; }

        public long Total { get; internal set; }

        public string Currency { get; internal set; }
    }
}