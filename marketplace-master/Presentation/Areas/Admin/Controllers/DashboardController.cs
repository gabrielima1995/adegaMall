using Application.Reports;
using Domain.DTO;
using Domain.Interfaces.Repositories;
using Presentation.Areas.Admin.Models.Reports;
using System;
using System.Web.Mvc;
using V2.Infrastructure.Modules.Metrics;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class DashboardController : Controller
    {
        private readonly IReportAppService _reportAppService;
        private readonly IMetricsQueries _metricsQueries;
        private readonly ICartRepository _cartRepository;

        public DashboardController(
            IReportAppService reportAppService,
            IMetricsQueries metricsQueries,
            ICartRepository cartRepository)
        {
            _reportAppService = reportAppService;
            _metricsQueries = metricsQueries;
            _cartRepository = cartRepository;
        }

        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
                startDate = new DateTime(2000, 1, 2);

            if (endDate == null)
                endDate = DateTime.Today.AddDays(1);


            var model = _reportAppService.GetAdminDashboardReport(new AdminDashboardReportRequest
            {
                StartDate = startDate ?? DateTime.Today.AddYears(-10),
                EndDate = endDate ?? DateTime.Today.AddDays(1D)
            });

            var viewModel = new AdminDashboardReportViewModel(model);

            if (Application.Config.MaaSConfig.Current.Dashboard.LevelTwo.Enabled)
            {
                var advancedMetricsResult = _metricsQueries.RunQuery(startDate, endDate);
                viewModel.AdvancedMetrics = advancedMetricsResult;
            }
            else
            {
                viewModel.AdvancedMetrics = new MetricsQueriesResultVM();
            }


            return View(viewModel);
        }

        public ActionResult Carts(DateTime? startDate, DateTime? endDate)
        {
            var list = _metricsQueries.RunCartsQuery(startDate, endDate);
            return View("Carts", list);
        }

        public ActionResult CartDetails(string buyerId)
        {
            if (buyerId == null)
                return HttpNotFound();

            var cart = _cartRepository.GetFull(buyerId);

            if (cart == null)
                return HttpNotFound();

            return View("CartDetails", cart);
        }
    }
}