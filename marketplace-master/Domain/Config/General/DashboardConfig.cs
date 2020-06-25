using Application.Config.Common;

namespace Application.Config.General
{
    public class DashboardConfig
    {
        public StatusConfig LevelOne { get; set; } = new StatusConfig(true);

        public StatusConfig LevelTwo { get; set; } = new StatusConfig(true);

        public DashboardSellerConfig SellerDashboard { get; set; } = new DashboardSellerConfig(true);
    }

    public class DashboardSellerConfig : StatusConfig
    {
        public DashboardSellerConfig(bool enabled) : base(enabled)
        {

        }

        public StatusConfig DateFilter { get; set; } = new StatusConfig();

        public StatusConfig QuotesTotal { get; set; } = new StatusConfig();

        public StatusConfig QuotesReceivedCount { get; set; } = new StatusConfig();

        public StatusConfig QuotesWonCount { get; set; } = new StatusConfig();

        public StatusConfig QuotesLostCount { get; set; } = new StatusConfig();

        public StatusConfig QuotesOpenCount { get; set; } = new StatusConfig();

        public StatusConfig ProposalCount { get; set; } = new StatusConfig();

        public StatusConfig OrdersTotal { get; set; } = new StatusConfig();

        public StatusConfig OrdersCount { get; set; } = new StatusConfig();

        public StatusConfig OrdersAverage { get; set; } = new StatusConfig();

        public StatusConfig UnitsSoldCount { get; set; } = new StatusConfig();

        public StatusConfig AmountInWarranty { get; set; } = new StatusConfig();
    }
}