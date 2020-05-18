using Domain.DTO;
using Domain.Entities.PortfolioModule;
using Domain.Entities.SubscriptionModule;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class AdminDashboardReport
    {
        public AdminDashboardReport()
        {
        }

        public AdminDashboardReport(AdminDashboardReportRequest request) : this()
        {
            StartDate = request.StartDate;
            EndDate = request.EndDate;
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IList<Seller> Sellers { get; set; } = new List<Seller>();

        public IList<Buyer> Buyers { get; set; } = new List<Buyer>();

        public IList<Quote> Quotes { get; set; } = new List<Quote>();

        public IList<Quote> ClosedQuotes { get; set; } = new List<Quote>();

        public IList<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public int CreditsPacksPurchased { get; set; }

        public decimal CreditsTransactionsTotal { get; set; }

        public SuccessfulOrderMetricsDto SuccessfulOrderMetrics { get; set; } = new SuccessfulOrderMetricsDto();

        public class SuccessfulOrderMetricsDto
        {
            public int Count { get; set; }
            public decimal Amount { get; set; }
            public decimal StartupFees { get; set; }
        }
    }
}
