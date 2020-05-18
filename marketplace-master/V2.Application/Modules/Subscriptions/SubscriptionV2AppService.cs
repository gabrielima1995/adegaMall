using Application.Config;
using Domain.Entities.SubscriptionModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Modules.Subscriptions;
using Framework;
using System;
using V2.Application.Modules.LeadSales;

namespace V2.Application.Modules.Subscriptions
{
    public class SubscriptionV2AppService
    {
        private readonly ILeadSalesAppService _leadSalesAppService;
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionV2AppService(
            ILeadSalesAppService leadSalesAppService,
            SubscriptionService subscriptionService)
        {
            _leadSalesAppService = leadSalesAppService;
            _subscriptionService = subscriptionService;
        }

        public OperationResult CheckCredits(string sellerId)
        {
            var subscription = _subscriptionService.Get(
                i => i.SellerId == sellerId,
                i => i.Plan);

            return CheckCredits(subscription);
        }

        public OperationResult CheckCredits(Subscription subscription)
        {
            if (!MaaSConfig.Current.Quote.LeadSales.Enabled) return OperationResult.Success;

            if (!subscription.IsActive) return OperationResult.Success;

            var plan = subscription.Plan;

            var credits = plan.CreditsPerMonth.GetValueOrDefault();
            var duration = plan.CreditsValidityDays.GetValueOrDefault(MaaSConfig.Current.Quote.LeadSales.CreditsDefaultDuration);

            if (credits == 0) return OperationResult.Success;

            if (subscription.NextCreditsAddDate.HasValue && subscription.NextCreditsAddDate.Value >= DateTime.UtcNow) return OperationResult.Success;

            var result = _leadSalesAppService.AddCredits(subscription.SellerId, credits, duration);

            if (result.OK)
            {
                _subscriptionService.Update(x => x.SellerId == subscription.SellerId, entity =>
                {
                    entity.Status = subscription.Status;
                    if (!subscription.NextCreditsAddDate.HasValue)
                    {
                        entity.LastCreditsAddDate = DateTime.UtcNow;
                        entity.NextCreditsAddDate = entity.LastCreditsAddDate.Value.AddMonths(1).ToEndOfDay().AddDays(-1);
                    }
                    else
                    {
                        entity.LastCreditsAddDate = subscription.NextCreditsAddDate;
                        entity.NextCreditsAddDate = entity.LastCreditsAddDate.Value.AddMonths(1);
                    }
                });
            }

            return result;
        }
        public Subscription GetSubscription(string userId)
        {
            var subscription = _subscriptionService.Get(i =>
                i.SellerId == userId,
                i => i.Plan);

            if (subscription != null)
                return subscription;

            return null;
        }
    }
}