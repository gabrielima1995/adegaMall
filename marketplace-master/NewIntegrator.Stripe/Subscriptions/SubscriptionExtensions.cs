using NewIntegrator.Stripe.Invoices;
using Stripe;
using System;

namespace NewIntegrator.Stripe.Subscriptions
{
    internal static class SubscriptionExtensions
    {
        public static StripeSubscription FromStripe(this Subscription subscription)
        {
            return new StripeSubscription
            {
                Id = subscription.Id,
                Status = subscription.EnumerationStatus(),
                LastInvoice = subscription.LatestInvoice?.FromStripe(),
                Created = subscription.Created ?? default(DateTime)
            };
        }

        public static StripeSubscriptionStatus EnumerationStatus(this Subscription subscription)
        {
            switch (subscription.Status)
            {
                case "incomplete":
                    return StripeSubscriptionStatus.Incomplete;

                case "incomplete_expired":
                    return StripeSubscriptionStatus.IncompleteExpired;

                case "trialing":
                    return StripeSubscriptionStatus.Trialing;

                case "active":
                    return StripeSubscriptionStatus.Active;

                case "past_due":
                    return StripeSubscriptionStatus.PastDue;

                case "canceled":
                    return StripeSubscriptionStatus.Canceled;

                case "unpaid":
                    return StripeSubscriptionStatus.Unpaid;

                default:
                    throw new ArgumentOutOfRangeException(nameof(subscription.Status));
            }
        }
    }
}