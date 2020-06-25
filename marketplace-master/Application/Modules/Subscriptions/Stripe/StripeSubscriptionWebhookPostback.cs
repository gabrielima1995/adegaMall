using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.Stripe
{
    public class StripeSubscriptionWebhookPostback : SubscriptionPostbackBase
    {
        private readonly StripeSubscriptionWebhookObject _o;

        public override SubscriptionStatus Status
        {
            get
            {
                switch (_o.status)
                {
                    case "canceled":
                        return SubscriptionStatus.Canceled;

                    case "past_due":
                    case "unpaid":
                        return SubscriptionStatus.Unpaid;

                    case "trialing":
                        return SubscriptionStatus.Trialing;

                    case "active":
                        return SubscriptionStatus.Paid;

                    case "incomplete":
                        return SubscriptionStatus.PendingPayment;

                    case "incomplete_expired":
                        return SubscriptionStatus.Refused;
                }

                return SubscriptionStatus.None;
            }
        }

        public override decimal TransactionAmount =>
            _o.plan.amount / 100M;

        public override DateTime SubscriptionDate =>
            DateTimeOffset.FromUnixTimeSeconds(_o.created).UtcDateTime;

        public override string GatewayId =>
            _o.id;

        public override string Currency =>
            _o.plan.currency;

        public override string PostbackUrl { get; } =
            string.Empty;

        public override string TransactionId =>
            _o.latest_invoice;

        public override string TransactionStatus =>
            string.Empty;

        public StripeSubscriptionWebhookPostback(StripeSubscriptionWebhookObject o)
        {
            _o = o;
        }
    }
}