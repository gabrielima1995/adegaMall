using Application.Config;
using Domain.Modules.Subscriptions;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.Linq;
using V2.Application.Modules.Subscriptions;

namespace V2.Application.Modules.LeadSales
{
    public class CheckSubscriptionCredits
    {
        private readonly ISubscriptionOperations _subscriptionOperations;
        private readonly SubscriptionService _subscriptionService;
        private readonly SubscriptionV2AppService _v2AppService;

        public CheckSubscriptionCredits(ISubscriptionOperations subscriptionOperations,
                                        SubscriptionService subscriptionService,
                                        SubscriptionV2AppService v2AppService)
        {
            _subscriptionOperations = subscriptionOperations;
            _subscriptionService = subscriptionService;
            _v2AppService = v2AppService;
        }

        public void RunJob()
        {
            if (!MaaSConfig.Current.Quote.LeadSales.Enabled)
                return;

            //Lists all subscriptions that have pending credit deposits
            var pendingSubscriptions = _subscriptionService.Query(i => i.Plan)
                                                           .Where(i => i.NextCreditsAddDate.HasValue && i.NextCreditsAddDate < DateTime.UtcNow).ToList();

            //Should remain true if all operations are successful
            bool success = true;

            foreach (var sub in pendingSubscriptions)
            {
                //Gets subscription status on payment gateway
                var statusResult = _subscriptionOperations.GetSubscriptionStatus(sub);

                if (!statusResult.OK) //Marks as failure if cannot get status
                {
                    success = false;
                    continue;
                }

                // set status
                sub.Status = statusResult.Result;

                //Deposit credits according to plan if subscription is active
                var result = _v2AppService.CheckCredits(sub);

                if (!result.OK) //Marks as failure if cannot get status
                {
                    success = false;
                }
            }

            if (!success)
                throw new Exception("One or more operations have failed"); //Force retry through Hangfire
        }
    }
}