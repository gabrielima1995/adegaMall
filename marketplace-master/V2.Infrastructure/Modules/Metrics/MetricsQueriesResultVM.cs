using System;
using System.Collections.Generic;

namespace V2.Infrastructure.Modules.Metrics
{
    public class MetricsQueriesResultVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int TotalGroupQuotes { get; set; } = 0;
        public int TotalIndividualQuotes { get; set; } = 0;

        public int CountAcceptedQuotes { get; set; } = 0;
        public int CountRejectedQuotes { get; set; } = 0;
        public decimal AverageAcceptedQuoteValue { get; set; } = 0;
        public List<MetricsQueriesItemVM> ListTopServiceSellers { get; set; } = new List<MetricsQueriesItemVM>();
        public List<MetricsQueriesItemVM> ListTopQuoteCategories { get; set; } = new List<MetricsQueriesItemVM>();

        public decimal AverageOrderValue { get; set; } = 0;
        public int CountAbandonedCarts { get; set; } = 0;
        public List<MetricsQueriesItemVM> ListTopProductSellers { get; set; } = new List<MetricsQueriesItemVM>();
        public List<MetricsQueriesItemVM> ListTopProducts { get; set; } = new List<MetricsQueriesItemVM>();
        public List<MetricsQueriesItemVM> ListTopProductCategories { get; set; } = new List<MetricsQueriesItemVM>();
    }

    public class MetricsQueriesItemVM
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Sales { get; set; }
        public decimal SalesTotal { get; set; }
    }

    public class MetricsQueriesSellerResultVM
    {
        public int QuotesCount { get; set; }
        public int AcceptedQuotes { get; set; }
        public int RejectedQuotes { get; set; }

        public decimal AcceptedQuotesTotal { get; set; }

        public int ProposalsSent { get; set; }

        public int OrdersCompleted { get; set; }
        public int ProductsSold { get; set; }

        public decimal TotalOrderRevenue { get; set; }

        public decimal TotalOrderLiquidRevenue { get; set; }

        public decimal AmountInWarranty { get; set; }

        public int TotalCredits { get; set; }
        public int NextExpirationCredits { get; set; }
        public DateTime? NextCreditsExpirationDate { get; set; }
        public DateTime? NextCreditsAddDate { get; set; }
        public int UnlockedQuotes { get; set; }
    }

    public class MetricsQueriesCartVM
    {
        public string BuyerName { get; set; }
        public string Email { get; set; }
        public string BuyerId { get; set; }
        public string CartId { get; set; }
        public decimal CartValue { get; set; }
        public DateTime CartDate { get; set; }
    }
}
