using Application.Config;
using Application.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Reports;

namespace Application.Reports
{
    public class ReportAppService : IReportAppService
    {
        private readonly ReportService _reportService;

        public ReportAppService(ReportService reportService)
        {
            _reportService = reportService;
        }

        public AdminDashboardReport GetAdminDashboardReport(AdminDashboardReportRequest request)
        {
            if (request == null)
            {
                request = new AdminDashboardReportRequest
                {
                    StartDate = DateHelper.FirstDayOfMonth,
                    EndDate = DateHelper.LastDayOfMonth
                };
            }
            else
            {
                request.EndDate = DateHelper.ToEndOfDay(request.EndDate);
            }

            request.RequireModerationBuyer = MaaSConfig.Current.Buyer.Moderation.Enabled;
            request.RequireModerationSeller = MaaSConfig.Current.Seller.Moderation.Enabled;
            request.ProductEnabled = MaaSConfig.Current.Product.Enabled;
            request.QuoteEnabled = MaaSConfig.Current.Quote.Enabled;
            request.SubscriptionEnabled = MaaSConfig.Current.Subscription.Enabled;
            request.CreditsPacksEnabled = MaaSConfig.Current.Quote.LeadSales.Enabled && MaaSConfig.Current.Quote.LeadSales.IndividualPacks.Enabled;

            return _reportService.GetAdminDashboardReport(request);
        }
    }
}
