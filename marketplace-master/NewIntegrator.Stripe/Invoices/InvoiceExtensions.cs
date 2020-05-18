using NewIntegrator.Stripe.PaymentIntents;
using Stripe;
using System;

namespace NewIntegrator.Stripe.Invoices
{
    internal static class InvoiceExtensions
    {
        public static StripeInvoice FromStripe(this Invoice invoice)
        {
            return new StripeInvoice
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Paid = invoice.Paid,
                PaymentIntent = invoice.PaymentIntent?.FromStripe(),
                Status = invoice.EnumerationStatus(),
                Currency = invoice.Currency,
                Total = invoice.Total
            };
        }

        public static StripeInvoiceStatus EnumerationStatus(this Invoice invoice)
        {
            switch (invoice.Status)
            {
                case "draft":
                    return StripeInvoiceStatus.Draft;

                case "open":
                    return StripeInvoiceStatus.Open;

                case "paid":
                    return StripeInvoiceStatus.Paid;

                case "uncollectible":
                    return StripeInvoiceStatus.Uncollectible;

                case "void":
                    return StripeInvoiceStatus.Void;

                default:
                    throw new ArgumentOutOfRangeException(nameof(invoice.Status));
            }
        }
    }
}