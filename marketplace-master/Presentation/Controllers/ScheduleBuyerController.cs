using Application.Config;
using Domain.Interfaces;
using System;
using System.Text;
using System.Web.Mvc;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers
{
    [AuthorizeBuyer]
    public class ScheduleBuyerController : BaseController
    {
        private MaaSConfig Config => MaaSConfig.Current;

        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IScheduleQueries _scheduleQueries;

        public ScheduleBuyerController(
            ITimeZoneProvider timeZoneProvider,
            IScheduleQueries scheduleQueries)
        {
            _timeZoneProvider = timeZoneProvider;
            _scheduleQueries = scheduleQueries;
        }

        // GET: Schedule
        public ActionResult Index(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.Buyer.Schedule.Enabled) return HttpNotFound();

            return View();
        }

        public ActionResult Events(DateTime start, DateTime end, Guid? productId, string lang)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startUtc = timeZone.ConvertTimeToUtc(start);
            var endUtc = timeZone.ConvertTimeToUtc(end);

            var buyerId = GetPersonId();
            var events = _scheduleQueries.ListRentalScheduleBuyerItems(productId, buyerId, startUtc, endUtc, timeZone);
            return Content(events.ToJson(), "application/json", Encoding.UTF8);
        }

    }
}