using Domain.Entities;
using Domain.Entities.SubscriptionModule;
using PaymentIntegrator.Core.Subscriptions;
using System.Collections.Generic;
using System.Linq;
using V2.Infrastructure.Modules.Metrics;

namespace Presentation.Areas.Admin.Models.Reports
{
    public class AdminDashboardReportViewModel
    {
        public AdminDashboardReport Model { get; set; }

        public AdminDashboardReportViewModel()
        {
            AdvancedMetrics = new MetricsQueriesResultVM();
        }

        public AdminDashboardReportViewModel(AdminDashboardReport model) : this()
        {
            Model = model;
        }

        public MetricsQueriesResultVM AdvancedMetrics { get; set; }

        public int NewSellersCount => Model?.Sellers.Count ?? 0;

        public int NewBuyersCount => Model?.Buyers.Count ?? 0;

        public int NewQuotesCount => Model?.Quotes.Count ?? 0;

        public int ActiveSubscriptionsCount => GetPayingSubscriptions().Count;

        public decimal ActiveSubscriptionsAmount => GetPayingSubscriptions().Sum(x => x.Amount);

        public int PaidOrdersCount => Model.SuccessfulOrderMetrics.Count;

        public decimal PaidOrdersAmount => Model.SuccessfulOrderMetrics.Amount;

        public decimal PaidOrdersFee => Model.SuccessfulOrderMetrics.StartupFees;

        public decimal TotalWonQuotesValue => Model.ClosedQuotes
            .Where(x => x.DealResult == Domain.QuoteRequests.QuoteDealResult.Won)
            .Sum(x => x.AcceptedPrice.GetValueOrDefault());

        public IList<Subscription> GetPayingSubscriptions()
        {
            if (Model == null || Model.Subscriptions.Count == 0)
                return new List<Subscription>();

            return Model.Subscriptions
                .Where(x => x.Status == SubscriptionStatus.Paid || x.Status == SubscriptionStatus.Trialing)
                .ToList();
        }
    }
}