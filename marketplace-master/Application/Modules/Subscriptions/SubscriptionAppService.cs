using Application.Config;
using Application.CreditCards;
using Domain.Entities;
using Domain.Entities.SubscriptionModule;
using Domain.Modules.Subscriptions;
using Domain.Modules.Subscriptions.Plans;
using Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using V2.Application.Modules.Subscriptions;

namespace Application.Modules.Subscriptions
{
    public class SubscriptionAppService
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly PlanService _planService;
        private readonly ISubscriptionGateway _subscriptionGateway;
        private readonly SubscriptionV2AppService _v2AppService;

        public SubscriptionAppService(
             SubscriptionService subscriptionService,
             PlanService planService,
             ISubscriptionGateway subscriptionGateway,
             SubscriptionV2AppService v2AppService)
        {
            _subscriptionService = subscriptionService;
            _planService = planService;
            _subscriptionGateway = subscriptionGateway;
            _v2AppService = v2AppService;
        }

        public IEnumerable<SubscriptionItemListViewModel> GetList()
        {
            var subscriptions = _subscriptionService
                .Query(
                    i => i.Seller,
                    i => i.Plan)
                .OrderByDescending(i => i.StartDate)
                .ToList();

            return subscriptions
                .Select(item => new SubscriptionItemListViewModel(item, item.Seller))
                .ToList();
        }

        public IEnumerable<SignModeViewModel> GetSignModesList(Person person, string lang)
        {
            IEnumerable<Plan> plans = _planService.GetPlans(true);

            // Se existem planos com trial e planos normais
            if (plans.Any(i => i.IsTrialPlan) && plans.Any(i => !i.IsTrialPlan))
            {
                plans = plans
                    .Where(i =>
                        // Se não é permitido para o usuário assinar planos trial, exibe somente planos que não tem período trial.
                        (person.NotAllowedTrial && !i.IsTrialPlan) ||

                        // Caso contrário, exibe somente os planos com período trial.
                        (!person.NotAllowedTrial && i.IsTrialPlan));
            }

            if (MaaSConfig.Current.Quote.LeadSales.Enabled)
                plans = plans.Where(x => x.CreditsPerMonth.HasValue);

            return plans
                .Select(plan => new SignModeViewModel(plan, lang)).OrderByDescending(x => x.PriceMonthly);
        }

        public SubscribeViewModel PrepareSubscribe(Guid planId, string lang)
        {
            var plan = _planService.Get(i => i.Id == planId);

            return plan != null ? new SubscribeViewModel(new SignModeViewModel(plan, lang)) : null;
        }

        public OperationResult Subscribe(string personId, Guid planId, CreditCardModel creditCard)
        {
            var plan = _planService.Query()
                .FirstOrDefault(i => i.Id == planId && i.Enabled);

            if (plan == null)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("PLAN_NOT_FOUND_MESSAGE"));

            var result = _subscriptionGateway.Subscribe(personId, plan, creditCard);

            if (!result.OK) return result;

            var subscription = result.Result;

            return _v2AppService.CheckCredits(subscription);
        }

        public OperationResult Unsubscribe(string userId)
        {
            return _subscriptionGateway.Unsubscribe(userId);
        }

        public SubscriptionViewModel GetSubscription(string userId)
        {
            var subscription = _subscriptionService.Get(i =>
                i.SellerId == userId,
                i => i.Plan);

            if (subscription != null)
                return new SubscriptionViewModel(subscription, CultureInfo.CurrentCulture.GetCurrentLang());

            return null;
        }
    }
}