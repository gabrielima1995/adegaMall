using NewIntegrator.Stripe.Subscriptions;
using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeSubscriptionPostback : SubscriptionPostbackBase
    {
        private readonly StripeSubscription _subscription;

        public override SubscriptionStatus Status
        {
            get
            {
                switch (_subscription.Status)
                {
                    case StripeSubscriptionStatus.Canceled:
                        return SubscriptionStatus.Canceled;

                    case StripeSubscriptionStatus.PastDue:
                    case StripeSubscriptionStatus.Unpaid:
                        return SubscriptionStatus.Unpaid;

                    case StripeSubscriptionStatus.Trialing:
                        return SubscriptionStatus.Trialing;

                    case StripeSubscriptionStatus.Active:
                        return SubscriptionStatus.Paid;

                    case StripeSubscriptionStatus.Incomplete:
                        return SubscriptionStatus.PendingPayment;

                    case StripeSubscriptionStatus.IncompleteExpired:
                        return SubscriptionStatus.Refused;

                    default:
                        return SubscriptionStatus.None;
                }
            }
        }

        public override decimal TransactionAmount =>
            _subscription.LastInvoice.Total / 100M;

        public override DateTime SubscriptionDate =>
            _subscription.Created;

        public override string GatewayId =>
            _subscription.Id;

        public override string Currency =>
            _subscription.LastInvoice.Currency;

        public override string PostbackUrl { get; } =
            string.Empty;

        public override string TransactionId =>
            _subscription.LastInvoice.Id;

        public override string TransactionStatus =>
            _subscription.LastInvoice.Status.ToString();

        public StripeSubscriptionPostback(StripeSubscription subscription)
        {
            _subscription = subscription;
        }
    }
}