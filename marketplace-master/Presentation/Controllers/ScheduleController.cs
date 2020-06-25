using Application.Config;
using Application.Modules.Products.Queries;
using Domain.Interfaces;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Presentation.Globalization;
using Presentation.Models;
using Presentation.Models.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.StoreModule.AppCommands;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers
{
    [AuthorizeSellerOrSellerAssistant]
    public class ScheduleController : BaseController
    {
        private MaaSConfig Config => MaaSConfig.Current;

        private readonly NewLockAppCommandHandler _newLockAppCommandHandler;
        private readonly DeleteLockAppCommandHandler _deleteLockAppCommandHandler;
        private readonly NewAvailabilityAppCommandHandler _availabilityAppCommandHandler;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IScheduleQueries _scheduleQueries;
        private readonly IScheduleTimelineQueries _scheduleTimelineQueries;
        private readonly IProductQueries _productQueries;
        private readonly IRentalLockRepository _rentalLockRepository;

        public ScheduleController(
            NewLockAppCommandHandler newLockAppCommandHandler,
            DeleteLockAppCommandHandler deleteLockAppCommandHandler,
            NewAvailabilityAppCommandHandler availabilityAppCommandHandler,
            ITimeZoneProvider timeZoneProvider,
            IScheduleQueries scheduleQueries,
            IScheduleTimelineQueries scheduleTimelineQueries,
            IProductQueries productQueries,
            IRentalLockRepository rentalLockRepository)
        {
            _newLockAppCommandHandler = newLockAppCommandHandler;
            _deleteLockAppCommandHandler = deleteLockAppCommandHandler;
            _availabilityAppCommandHandler = availabilityAppCommandHandler;
            _timeZoneProvider = timeZoneProvider;
            _scheduleQueries = scheduleQueries;
            _scheduleTimelineQueries = scheduleTimelineQueries;
            _productQueries = productQueries;
            _rentalLockRepository = rentalLockRepository;
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleView)]
        public ActionResult Index(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            var sellerId = GetPersonId();
            var productsWithRentals = _scheduleQueries.ListProductsWithRentalsAndLocks(sellerId);
            ViewBag.ProductId = new SelectList(productsWithRentals, "Id", "Title", productId);

            return View();
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult NewLock()
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            SetViewBagForNewLock();

            return View(new NewLockVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult NewLock(NewLockVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetCommand(model);
                var commandResult = _newLockAppCommandHandler.Handle(command);
                if (commandResult.OK) return RedirectToAction("Index", new { lang = Request.GetCurrentLang() });

                ModelState.AddErrors(commandResult);
            }

            SetViewBagForNewLock();

            return View(model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult NewAvailability(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            SetViewBagForNewAvailability();

            var sellerId = GetPersonId();
            var locks = _rentalLockRepository.GetDayOfWeeksLocks(sellerId, productId);

            return View(NewAvailabilityVM.Prepare(productId, locks));
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult NewAvailabilityDaysOfWeek(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            var sellerId = GetPersonId();
            var locks = _rentalLockRepository.GetDayOfWeeksLocks(sellerId, productId);

            return PartialView("_NewAvailabilityDaysOfWeekForm", NewAvailabilityVM.Prepare(productId, locks));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult NewAvailability(NewAvailabilityVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetCommand(model);
                var commandResult = _availabilityAppCommandHandler.Handle(command);
                if (commandResult.OK) return RedirectToAction("Index", new { lang = Request.GetCurrentLang() });

                ModelState.AddErrors(commandResult);
            }

            SetViewBagForNewAvailability();

            return View(model);
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult LockNightBeforeMidnight(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            var sellerId = GetPersonId();
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startTime = TimeSpan.Parse("18:00");
            var endTime = TimeSpan.Parse("23:59");

            NewLockAppCommand NewLockByDayOfWeek(DayOfWeek dayOfWeek)
            {
                return NewLockAppCommand.NewLockByDayOfWeek(sellerId, timeZone,
                    dayOfWeek, startTime, endTime,
                    LockMode.OnlyForShipping, productId);
            }

            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Sunday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Monday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Tuesday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Wednesday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Thursday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Friday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Saturday));

            return RedirectToAction("Index", new { productId });
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult LockNightAfterMidnight(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            var sellerId = GetPersonId();
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startTime = TimeSpan.Parse("00:00");
            var endTime = TimeSpan.Parse("07:59");

            NewLockAppCommand NewLockByDayOfWeek(DayOfWeek dayOfWeek)
            {
                return NewLockAppCommand.NewLockByDayOfWeek(sellerId, timeZone,
                    dayOfWeek, startTime, endTime,
                    LockMode.OnlyForShipping, productId);
            }

            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Sunday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Monday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Tuesday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Wednesday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Thursday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Friday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Saturday));

            return RedirectToAction("Index", new { productId });
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult LockWeekend(Guid? productId = null)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled) return HttpNotFound();

            var sellerId = GetPersonId();
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startTime = TimeSpan.Parse("00:00");
            var endTime = TimeSpan.Parse("23:59");

            NewLockAppCommand NewLockByDayOfWeek(DayOfWeek dayOfWeek)
            {
                return NewLockAppCommand.NewLockByDayOfWeek(sellerId, timeZone,
                    dayOfWeek, startTime, endTime,
                    LockMode.OnlyForShipping, productId);
            }

            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Saturday));
            _newLockAppCommandHandler.Handle(NewLockByDayOfWeek(DayOfWeek.Sunday));

            return RedirectToAction("Index", new { productId });
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleEdit)]
        public ActionResult DeleteLock(Guid id)
        {
            if (ModelState.IsValid)
            {
                var command = new DeleteLockAppCommand
                {
                    SellerId = GetPersonId(),
                    LockId = id
                };

                var commandResult = _deleteLockAppCommandHandler.Handle(command);

                if (commandResult.OK)
                    this.SetSuccessMessage(Request.GetLocalizedString("DELETE_LOCK_SUCCESS_MESSAGE"), true);
                else
                    this.SetErrorMessage(Request.GetLocalizedString("DELETE_LOCK_ERROR_MESSAGE"), true);
            }

            return RedirectToAction("Index", new { lang = Request.GetCurrentLang() });
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleView)]
        public ActionResult Events(DateTime start, DateTime end, Guid? productId, string lang)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startUtc = start.Kind == DateTimeKind.Local ? start.ToUniversalTime() : timeZone.ConvertTimeToUtc(start);
            var endUtc = end.Kind == DateTimeKind.Local ? end.ToUniversalTime() : timeZone.ConvertTimeToUtc(end);

            var events = _scheduleQueries.ListRentalScheduleItems(productId, GetPersonId(), startUtc, endUtc, timeZone);
            return Content(events.ToJson(), "application/json", Encoding.UTF8);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleView)]
        public ActionResult TimelineResources(Guid? productId, string lang)
        {
            var sellerId = GetPersonId();

            var resources = _scheduleTimelineQueries.ListRentalScheduleTimelineResources(productId, sellerId);

            var storeTitle = Request.GetLocalizedString("SCHEDULE_TIMELINE_STORE_RESOURCE_TITLE");

            resources.Insert(0, RentalScheduleTimelineResourceItem.Store(storeTitle));

            return Content(resources.ToJson(), "application/json", Encoding.UTF8);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleView)]
        public ActionResult TimelineEvents(DateTime start, DateTime end, Guid? productId, string lang)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startUtc = start.Kind == DateTimeKind.Local ? start.ToUniversalTime() : timeZone.ConvertTimeToUtc(start);
            var endUtc = end.Kind == DateTimeKind.Local ? end.ToUniversalTime() : timeZone.ConvertTimeToUtc(end);

            var events = _scheduleTimelineQueries.ListRentalScheduleTimelineItems(productId, GetPersonId(), startUtc, endUtc, timeZone);
            return Content(events.ToJson(), "application/json", Encoding.UTF8);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ScheduleView)]
        public ActionResult TimelineRentals(DateTime start, DateTime end, Guid? productId, Guid? productAvailability, string lang)
        {
            var timeZone = _timeZoneProvider.GetTimeZone();
            var startUtc = start.Kind == DateTimeKind.Local ? start.ToUniversalTime() : timeZone.ConvertTimeToUtc(start);
            var endUtc = end.Kind == DateTimeKind.Local ? end.ToUniversalTime() : timeZone.ConvertTimeToUtc(end);

            var rentals = _scheduleQueries.ListRentalScheduleItems(productId, GetPersonId(), startUtc, endUtc, timeZone, true, productAvailability);

            rentals = rentals.Where(i => i.StartUTC == startUtc && i.EndUTC == endUtc).ToList();

            return PartialView("_TimelineRentals", rentals);
        }

        private NewLockAppCommand GetCommand(NewLockVM model)
        {
            return new NewLockAppCommand
            {
                SellerId = GetPersonId(),
                Type = model.Type,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ProductId = model.ProductId == null ? default(Guid?) : Guid.Parse(model.ProductId),
                TimeZone = _timeZoneProvider.GetTimeZone(),
                Mode = model.Mode
            };
        }

        private NewAvailabilityAppCommand GetCommand(NewAvailabilityVM model)
        {
            var command = NewAvailabilityAppCommand.New(GetPersonId(), _timeZoneProvider.GetTimeZone(),
                model.ProductId);

            foreach (var item in model.DaysOfWeek)
            {
                command.AddDayOfWeek(item.DayOfWeek,
                    item.StartTime1, item.EndTime1,
                    item.StartTime2, item.EndTime2);
            }

            return command;
        }

        private void SetViewBagForNewLock()
        {
            ViewBag.DaysOfWeek = new SelectList(new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Sunday, "Domingo" },
                { DayOfWeek.Monday, "Segunda" },
                { DayOfWeek.Tuesday, "Terça" },
                { DayOfWeek.Wednesday, "Quarta" },
                { DayOfWeek.Thursday, "Quinta" },
                { DayOfWeek.Friday, "Sexta" },
                { DayOfWeek.Saturday, "Sábado" }
            }, "Key", "Value");

            ViewBag.Types = new SelectList(new Dictionary<LockType, string>
            {
                { LockType.ByDate, "Por data" },
                { LockType.ByDayOfWeek, "Por dia da semama" }
            }, "Key", "Value");

            SetViewBagProducts();
        }

        private void SetViewBagForNewAvailability()
        {
            SetViewBagProducts();
        }

        private void SetViewBagProducts()
        {
            ViewBag.Products = new SelectList(_productQueries.ListForSelector(GetPersonId()), "Id", "Title");
        }
    }
}