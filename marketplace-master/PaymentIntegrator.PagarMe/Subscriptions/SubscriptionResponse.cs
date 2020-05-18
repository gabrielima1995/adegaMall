using PagarMe;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;
using PaymentIntegrator.PagarMe.Customers;
using PaymentIntegrator.PagarMe.Payments;
using PaymentIntegrator.PagarMe.Plans;
using SubscriptionStatus = PaymentIntegrator.Core.Subscriptions.SubscriptionStatus;

namespace PaymentIntegrator.PagarMe.Subscriptions
{
    /// <summary>
    /// Representa uma assinatura já salva no Pagar.me.
    /// </summary>
    public class SubscriptionResponse : ISubscription
    {
        public SubscriptionStatus Status { get; private set; }

        public ICustomer Customer { get; private set; }

        public IPlan Plan { get; private set; }

        public ITransaction CurrentTransaction { get; private set; }

        public string PostbackUrl { get; private set; }

        public string GatewayId { get; private set; }

        public SubscriptionResponse(Subscription subscription)
        {
            Status = (SubscriptionStatus)subscription.Status.GetHashCode();

            Customer = new CustomerResponse(subscription.Customer);
            Plan = new PlanResponse(subscription.Plan);

            if (subscription.CurrentTransaction != null)
            {
                CurrentTransaction = new TransactionResponse(subscription.CurrentTransaction);
            }

            PostbackUrl = subscription.PostbackUrl;
            GatewayId = subscription.Id;
        }

        public void SetCurrentStatus(SubscriptionStatus status)
        {
            Status = status;
        }
    }
}