using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;

namespace PaymentIntegrator.Moip.Subscriptions
{
    public class MoipSubscription : ISubscription
    {
        public ITransaction CurrentTransaction { get; set; }
        public ICustomer Customer { get; set; }
        public string GatewayId { get; set; }
        public IPlan Plan { get; set; }
        public string PostbackUrl { get; set; }
        public SubscriptionStatus Status { get; set; }

        public static SubscriptionStatus GetStatus(string status)
        {
            switch (status.ToUpperInvariant())
            {
                case "SUSPENDED":
                case "EXPIRED":
                    return SubscriptionStatus.Ended;

                case "ACTIVE":
                    return SubscriptionStatus.Paid;

                case "TRIAL":
                    return SubscriptionStatus.Trialing;

                case "CANCELED":
                    return SubscriptionStatus.Canceled;

                case "OVERDUE":
                default:
                    return SubscriptionStatus.Unpaid;
            }
        }

        public void SetCurrentStatus(SubscriptionStatus status)
        {
            Status = status;
        }

        public void SetCurrentStatus(string status)
        {
            switch (status.ToUpperInvariant())
            {
                case "ACTIVE":
                    Status = SubscriptionStatus.Paid;
                    break;

                case "TRIAL":
                    Status = SubscriptionStatus.Trialing;
                    break;

                case "OVERDUE":
                    Status = SubscriptionStatus.Unpaid;
                    break;

                case "CANCELED":
                    Status = SubscriptionStatus.Canceled;
                    break;

                case "EXPIRED":
                    Status = SubscriptionStatus.Unpaid;
                    break;

                default:
                    break;
            }
        }
    }
}