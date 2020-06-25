using Application.Reports;
using Domain.DTO;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.WebHelpers;
using Presentation.Areas.Admin.Models.Reports;
using System;
using System.Configuration;
using System.Web.Http;
using V2.Infrastructure.Modules.Metrics;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/metrics")]
    public class MetricsApiV1Controller : ApiController
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMetricsQueries _metricsQueries;
        private readonly IReportAppService _reportAppService;

        public MetricsApiV1Controller(
            IReportAppService reportAppService,
            IMetricsQueries metricsQueries,
            ICartRepository cartRepository)
        {
            _reportAppService = reportAppService;
            _metricsQueries = metricsQueries;
            _cartRepository = cartRepository;
        }

        [Route("")]
        public IHttpActionResult Get(DateTime? start_date, DateTime? end_date)
        {
            if (start_date == null)
                start_date = new DateTime(2000, 1, 2);

            if (end_date == null)
                end_date = DateTime.Today.AddDays(1);

            var model = _reportAppService.GetAdminDashboardReport(new AdminDashboardReportRequest
            {
                StartDate = start_date ?? DateTime.Today.AddYears(-10),
                EndDate = end_date ?? DateTime.Today.AddDays(1D)
            });

            var viewModel = new AdminDashboardReportViewModel(model);
            var isInaAdmin = AuthWebHelper.GetAuthToken() == ConfigurationManager.AppSettings["INAMASTERAPITOKEN"];

            if (isInaAdmin || Application.Config.MaaSConfig.Current.Dashboard.LevelTwo.Enabled)
            {
                var advancedMetricsResult = _metricsQueries.RunQuery(start_date, end_date);
                viewModel.AdvancedMetrics = advancedMetricsResult;
            }

            return Ok(new
            {
                start_date,
                end_date,
                countBuyers = viewModel.Model.Buyers.Count,
                countOrders = viewModel.Model.SuccessfulOrderMetrics.Count,
                coutProducts = viewModel.AdvancedMetrics.ListTopProducts,
                countQuotes = viewModel.Model.Quotes.Count,
                countSellers = viewModel.Model.Sellers.Count,
                countSubscriptions = viewModel.Model.Subscriptions.Count,
                viewModel.NewBuyersCount,
                viewModel.NewQuotesCount,
                viewModel.NewSellersCount,
                viewModel.PaidOrdersCount,
                viewModel.PaidOrdersAmount,
                viewModel.AdvancedMetrics.AverageAcceptedQuoteValue,
                viewModel.AdvancedMetrics.CountRejectedQuotes,
                viewModel.AdvancedMetrics.CountAcceptedQuotes,
                viewModel.AdvancedMetrics.TotalIndividualQuotes,
                viewModel.AdvancedMetrics.TotalGroupQuotes,
                viewModel.AdvancedMetrics.CountAbandonedCarts,
                viewModel.ActiveSubscriptionsAmount,
                viewModel.ActiveSubscriptionsCount,
                viewModel.TotalWonQuotesValue
            });
        }
    }
}