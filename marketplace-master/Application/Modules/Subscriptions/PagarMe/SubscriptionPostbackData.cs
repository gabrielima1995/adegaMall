using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Dados do objeto Subscription no postback enviado pelo Pagar.me.
    /// </summary>
    public class SubscriptionPostbackData
    {
        public string id { get; set; }

        public string postback_url { get; set; }

        public string payment_method { get; set; }

        public DateTime? current_period_start { get; set; }

        public DateTime? current_period_end { get; set; }

        public int charges { get; set; }

        public string status { get; set; }

        public DateTime date_created { get; set; } = DateTime.UtcNow;

        public PlanPostbackData plan { get; set; }

        public TransactionPostbackData current_transaction { get; set; }

        public CustomerPostbackData customer { get; set; }

        public CardPostbackData card { get; set; }

        public SubscriptionPostbackData()
        {
        }

        public SubscriptionPostbackData(ISubscription subscription)
        {
            id = subscription.GatewayId;
            postback_url = subscription.PostbackUrl;
            plan = new PlanPostbackData(subscription.Plan);

            if (subscription.CurrentTransaction != null)
            {
                current_transaction = new TransactionPostbackData(subscription.CurrentTransaction);
            }

            customer = new CustomerPostbackData(subscription.Customer);
        }
    }
}