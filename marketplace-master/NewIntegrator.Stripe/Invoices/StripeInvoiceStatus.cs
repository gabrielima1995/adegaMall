namespace NewIntegrator.Stripe.Invoices
{
    public enum StripeInvoiceStatus
    {
        Draft,
        Open,
        Paid,
        Uncollectible,
        Void
    }
}