using NewIntegrator.Stripe.Invoices;
using System;

namespace NewIntegrator.Stripe.Subscriptions
{
    public class StripeSubscription
    {
        public string Id { get; internal set; }

        public StripeSubscriptionStatus Status { get; internal set; }

        public StripeInvoice LastInvoice { get; internal set; }

        public DateTime Created { get; internal set; }
    }
}