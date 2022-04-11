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
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers.Products
{
    [AuthorizeSellerOrSellerAssistant]
    public class MySalesController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Products/MySales/{0}.cshtml";

        private readonly ITimeZoneProvider userTimeZoneProvider;
        private readonly OrderService orderService;
        private readonly IScheduledTransferRepository scheduledTransferRepository;
        private readonly OrderAppService orderAppService;
        private readonly IOrderQueries orderQueries;
        private readonly IUserRatingsRepository userRatingsRepository;
        private readonly IRentalShiftQueries rentalShiftQueries;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly BuyerService buyerService;
        private readonly ICustomFieldQueries customFieldQueries;
        private readonly ICustomFieldRepository customFieldRepository;

        public MySalesController(
            ITimeZoneProvider userTimeZoneProvider,
            OrderService orderService,
            OrderAppService orderAppService,
            IOrderQueries orderQueries,
            ICustomFieldAppService customFieldAppService,
            IScheduledTransferRepository scheduledTransferRepository,
            IUserRatingsRepository userRatingsRepository,
            IRentalShiftQueries rentalShiftQueries,
            BuyerService buyerService,
            ICustomFieldQueries customFieldQueries,
            ICustomFieldRepository customFieldRepository)
        {
            this.userTimeZoneProvider = userTimeZoneProvider;
            this.orderService = orderService;
            this.orderAppService = orderAppService;
            this.orderQueries = orderQueries;
            this.scheduledTransferRepository = scheduledTransferRepository;
            this.userRatingsRepository = userRatingsRepository;
            this.rentalShiftQueries = rentalShiftQueries;
            this.customFieldAppService = customFieldAppService;
            this.buyerService = buyerService;
            this.customFieldQueries = customFieldQueries;
            this.customFieldRepository = customFieldRepository;
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.SalesView)]
        public ActionResult Index(bool showCanceled = false)
        {
            var model = orderAppService.GetMySales(GetPersonId(), showCanceled);
            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.SalesView)]
        public ActionResult Details(Guid id)
        {
            var details = orderQueries.GetOrderDetailsForBuyerAndSeller(id);
            var userId = GetPersonId();
            var timeZone = userTimeZoneProvider.GetTimeZone();
            var buyer = buyerService.GetById(details.Order.Buyer.Id);
            var buyerVM = new BuyerProfileVM(buyer);

            if (details == null || details.Order.Seller.Id != userId)
                return this.NotFound();

            var userRating = MaaSConfig.Current.Buyer.UserRatings.Enabled
                ? userRatingsRepository.GetByOrderId(details.Order.Id.ToString(), details.Order.Buyer.Id)
                : null;

            var viewModel = new OrderDetailsVM(orderService, details, userRating, timeZone, true, buyerVM);

            if (MaaSConfig.Current.CustomFields.Buyer.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(buyerVM);

            if (MaaSConfig.Current.CustomFields.CartItem.Enabled)
            {
                viewModel.CustomFieldDefinitions = customFieldRepository.DictionaryByEntityType(Domain.Entities.CustomFieldEntityType.CartItem);
                viewModel.CustomFields.AddRange(customFieldQueries.ReadValuesForEntityId(Domain.Entities.CustomFieldEntityType.CartItem, id));
                if (viewModel.CustomFields != null || viewModel.CustomFields.Count != 0)
                    viewModel.CustomFieldsNames = viewModel.CustomFields.DistinctBy(x => x.Name).Select(x => x.Name).ToList();
            }

            var transfer = scheduledTransferRepository.Get(x => x.OrderId == id.ToString());
            ViewBag.TransferAuthorized = transfer != null && transfer.TransferAuthorized;

            return View(string.Format(ViewPathFmt, "Details"), viewModel);
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.SalesEdit)]
        public ActionResult ChangeStatus(Guid id, OrderStatus status, string additionalInfo = null)
        {
            if (status == OrderStatus.Delivered)
                status = OrderStatus.Completed;

            var result = orderAppService.ChangeStatusBySeller(GetPersonId(), id, status, additionalInfo);

            if (result.IsFailure)
                this.SetErrorMessage(result.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("ORDER_CHANGE_STATUS_MESSAGE"), true);

            return RedirectToAction("Details", new { id, lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.SalesEdit)]
        public ActionResult Review(OrderReviewViewModel model)
        {
            model.ReviewerId = GetPersonId();
            var validation = orderAppService.Review(model);

            if (validation.IsFailure)
                this.SetErrorMessage(validation.Errors.ToSimpleString(), true);
            else this.SetSuccessMessage(Request.GetLocalizedString("ORDER_REVIEW_MESSAGE"), true);

            return RedirectToAction("Details", new { id = model.OrderId, lang = Request.GetCurrentLang() });
        }
    }
}