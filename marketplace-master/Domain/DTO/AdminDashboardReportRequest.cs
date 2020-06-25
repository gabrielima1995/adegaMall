using System;

namespace Domain.DTO
{
    public class AdminDashboardReportRequest
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool ProductEnabled { get; set; }

        public bool QuoteEnabled { get; set; }

        public bool SubscriptionEnabled { get; set; }

        public bool CreditsPacksEnabled { get; set; }

        public bool RequireModerationBuyer { get; set; }

        public bool RequireModerationSeller { get; set; }
    }
}
