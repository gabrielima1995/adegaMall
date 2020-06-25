using Domain.Interfaces;
using Domain.Services.StoreModule;
using Presentation.Areas.Admin.Models;
using Presentation.Models.FinanceViewModels;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules.Finance.Queries;

namespace Presentation.Areas.Admin.Controllers.Products
{
    [AuthorizeAdmin]
    public class OrdersController : Controller
    {
        private readonly OrderService orderService;
        private readonly ITransferJournalQueries transferJournalQueries;
        private readonly ITimeZoneProvider timeZoneProvider;

        public OrdersController(
            OrderService orderService,
            ITransferJournalQueries transferJournalQueries,
            ITimeZoneProvider timeZoneProvider)
        {
            this.orderService = orderService;
            this.transferJournalQueries = transferJournalQueries;
            this.timeZoneProvider = timeZoneProvider;
        }

        public ActionResult Index()
        {
            var timeZone = timeZoneProvider.GetTimeZone();
            var items = orderService.ListForAdmin();
            var viewModel = items.Select(x => new OrderListItemVM(x, timeZone)).ToList();

            return View(viewModel);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null) return HttpNotFound();

            var order = orderService.GetDetails(id.Value);
            if (order == null) return HttpNotFound();

            var timeZone = timeZoneProvider.GetTimeZone();
            var viewModel = new OrderDetailsViewModel(order, timeZone);

            return View(viewModel);
        }

        public ActionResult CheckTranStatus(Guid? id)
        {
            if (id == null) return HttpNotFound();
            var details = orderService.GetDetails(id.Value);
            var result = orderService.ProcessGatewayPostback(details.Order.Transaction.TransactionId);

            return RedirectToAction("Details", new { id });
        }

        public ActionResult TransferJournal(string period)
        {
            DateTime startDate, endDate = DateTime.MaxValue;

            if (period == null) period = Request["transfer_journal_period"];
            var periodCookie = new HttpCookie("transfer_journal_period", period)
            {
                Expires = DateTime.Today.AddDays(30D)
            };

            switch (period)
            {
                case "seven_days":
                    startDate = DateTime.Today.AddDays(-7D);
                    break;

                case "thirty_days":
                    startDate = DateTime.Today.AddDays(-30D);
                    break;

                case "current_month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;

                default:
                    periodCookie.Value = "all";
                    startDate = new DateTime(1753, 1, 1);
                    break;
            }

            var param = new TransferJournalReportParameters(null, 0, int.MaxValue, startDate: startDate, endDate: endDate);
            var report = transferJournalQueries.GetReport(param);
            var viewModel = new TransferJournalReportVM(report);

            Response.SetCookie(periodCookie);
            return View(viewModel);
        }
    }
}