using Application.Config;
using Application.Modules.Products.Orders;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings;
using Domain.Services;
using Domain.Services.StoreModule;
using Domain.StoreModule.Interfaces;
using Presentation.Globalization;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers.Products
{
    [AuthorizeBuyer]
    public class MyOrdersController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Products/MyOrders/{0}.cshtml";

        private readonly ITimeZoneProvider userTimeZoneProvider;
        private readonly OrderService orderService;
        private readonly OrderAppService orderAppService;
        private readonly IOrderQueries orderQueries;
        private readonly IUserRatingsRepository userRatingsRepository;
        private readonly IRentalShiftQueries rentalShiftQueries;
        private readonly BuyerService buyerService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ICustomFieldQueries customFieldQueries;
        private readonly ICustomFieldRepository customFieldRepository;

        public MyOrdersController(
            ITimeZoneProvider userTimeZoneProvider,
            OrderService orderService,
            OrderAppService orderAppService,
            IOrderQueries orderQueries,
            IUserRatingsRepository userRatingsRepository,
            IRentalShiftQueries rentalShiftQueries,
            ICustomFieldAppService customFieldAppService,
            ICustomFieldQueries customFieldQueries,
            ICustomFieldRepository customFieldRepository,
            BuyerService buyerService)
        {
            this.userTimeZoneProvider = userTimeZoneProvider;
            this.orderService = orderService;
            this.orderAppService = orderAppService;
            this.orderQueries = orderQueries;
            this.userRatingsRepository = userRatingsRepository;
            this.rentalShiftQueries = rentalShiftQueries;
            this.customFieldAppService = customFieldAppService;
            this.customFieldQueries = customFieldQueries;
            this.customFieldRepository = customFieldRepository;
            this.buyerService = buyerService;
        }

        public ActionResult Index()
        {
            var buyerId = GetPersonId();
            var model = orderAppService.GetMyOrders(buyerId);
            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        public ActionResult Details(Guid id)
        {
            var buyerId = GetPersonId();
            var details = orderQueries.GetOrderDetailsForBuyerAndSeller(id);
            var timeZone = userTimeZoneProvider.GetTimeZone();
            var buyer = buyerService.GetById(details.Order.Buyer.Id);
            var buyerVM = new BuyerProfileVM(buyer);

            if (details == null || details.Order.Buyer.Id != buyerId)
                return this.NotFound();

            var userRating = MaaSConfig.Current.Seller.UserRatings.Enabled
                ? userRatingsRepository.GetByOrderId(details.Order.OrderId, details.Order.Seller.Id)
                : null;

            var viewModel = new OrderDetailsVM(orderService, details, userRating, timeZone, false, buyerVM);

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(buyerVM);

            if (MaaSConfig.Current.CustomFields.CartItem.Enabled)
            {
                viewModel.CustomFieldDefinitions = customFieldRepository.DictionaryByEntityType(viewModel.EntityType);
                viewModel.CustomFields = customFieldQueries.ReadValuesForEntityId(Domain.Entities.CustomFieldEntityType.CartItem, id);
                if (viewModel.CustomFields != null || viewModel.CustomFields.Count != 0)
                    viewModel.CustomFieldsNames = viewModel.CustomFields.DistinctBy(x => x.Name).Select(x => x.Name).ToList();
            }

            return View(string.Format(ViewPathFmt, "Details"), viewModel);
        }

        [HttpPost]
        public ActionResult Review(OrderReviewViewModel model)
        {
            model.ReviewerId = GetPersonId();
            var reviewResult = orderAppService.Review(model);

            if (reviewResult.IsFailure)
                this.SetErrorMessage(reviewResult.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("ORDER_REVIEW_MESSAGE"), true);

            return RedirectToAction("Details", new { id = model.OrderId, lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult ChangeStatus(Guid id) /*este método é usado para CANCELAMENTO do agendamento*/
        {
            var buyerId = GetPersonId();
            var result = orderAppService.ChangeStatusByBuyer(buyerId, id, OrderStatus.Canceled, Request.GetLocalizedString("ORDER_BUYER_CANCEL_ADDITIONAL_INFO"));

            if (result.IsFailure)
                this.SetErrorMessage(result.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("ORDER_CHANGE_STATUS_MESSAGE"), true);

            return RedirectToAction("Details", new { id, lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult ChangeRentalProductPeriod(ChangeRentalProductPeriodViewModel model)
        {
            var buyerId = GetPersonId();
            var timeZone = userTimeZoneProvider.GetTimeZone();
            var result = orderAppService.ChangeRentalProductPeriodByBuyer(buyerId, model, timeZone);

            if (result.IsFailure)
                this.SetErrorMessage(result.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("RENTAL_PERIOD_CHANGED_MESSAGE"), true);

            return RedirectToAction("Details", new { id = model.OrderId, lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        public ActionResult NewRentalProductPeriod(NewRentalProductPeriodViewModel model)
        {
            var buyerId = GetPersonId();
            var timeZone = userTimeZoneProvider.GetTimeZone();
            var result = orderAppService.NewRentalProductPeriodByBuyer(buyerId, model, timeZone);

            if (result.IsFailure)
                this.SetErrorMessage(result.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("NEW_RENTAL_PERIOD_CREATED_MESSAGE"), true);

            return RedirectToAction("Details", new { id = model.OrderId, lang = Request.GetCurrentLang() });
        }
    }
}