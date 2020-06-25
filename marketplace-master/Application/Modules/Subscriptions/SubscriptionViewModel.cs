using Domain.Entities.SubscriptionModule;
using Infra.Crosscutting.Globalization;
using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Dados da página de asinatura
    /// </summary>
    public class SubscriptionViewModel
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public string CurrencySymbol { get; set; }

        public DateTime EnabledAt { get; set; }

        public DateTime? NextCreditsAddDate { get; set; }

        public int? CreditsPerMonth { get; set; }

        public DateTime? DateToFinishSubscription { get; set; }

        public SubscriptionViewModel()
        {
        }

        public SubscriptionViewModel(Subscription subscription, string lang)
        {
            Title = GlobalizationHelper.GetString(subscription.Plan.NameLabel, lang);
            Amount = subscription.Amount;
            CurrencySymbol = subscription.Currency;
            EnabledAt = subscription.StartDate;
            NextCreditsAddDate = subscription.NextCreditsAddDate;
            CreditsPerMonth = subscription.Plan.CreditsPerMonth;
            DateToFinishSubscription = ReturnFinishDate(subscription);
        }

        private DateTime ReturnFinishDate(Subscription subscription)
        {
            if(subscription.Status == SubscriptionStatus.Trialing)
            {
                return subscription.StartDate.AddDays(subscription.Plan.TrialDays);
            }
            else
            {
                return subscription.StartDate.AddDays(subscription.Plan.Days);
            }
        }
    }
}