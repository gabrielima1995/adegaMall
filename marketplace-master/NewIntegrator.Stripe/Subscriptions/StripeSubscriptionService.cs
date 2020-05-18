using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.Subscriptions
{
    public class StripeSubscriptionService
    {
        private static readonly string[] ValidStatuses =
        {
            "incomplete",
            "trialing",
            "active"
        };

        public async Task<StripeSubscription> CreateAsync(string customerId, string planId)
        {
            var items = new List<SubscriptionItemOption>
            {
                new SubscriptionItemOption
                {
                    PlanId = planId
                }
            };

            var options = new SubscriptionCreateOptions
            {
                CustomerId = customerId,
                Items = items,
                Expand = new List<string>
                {
                    "latest_invoice.payment_intent"
                }
            };

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            return subscription.FromStripe();
        }

        public StripeSubscription GetByCustomer(string customerId, bool onlyValid = true)
        {
            var service = new SubscriptionService();

            var options = new SubscriptionListOptions
            {
                Limit = 3,
                Expand = new List<string>
                {
                    "data.latest_invoice.payment_intent"
                }
            };

            var response = service.List(options);

            if (response.Data != null && response.Data.Any())
            {
                return response
                    .OrderByDescending(i => i.Created)
                    .FirstOrDefault(i => !onlyValid || ValidStatuses.Contains(i.Status))
                    .FromStripe();
            }

            return null;
        }

        public StripeSubscription Get(string subscriptionId)
        {
            var service = new SubscriptionService();

            var options = new SubscriptionGetOptions
            {
                Expand = new List<string>
                {
                    "latest_invoice.payment_intent"
                }
            };

            var subscription = service.Get(subscriptionId, options);

            return subscription.FromStripe();
        }

        public void Cancel(string subscriptionId)
        {
            var service = new SubscriptionService();

            var options = new SubscriptionCancelOptions();

            service.Cancel(subscriptionId, options);
        }
    }
}