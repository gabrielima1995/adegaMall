using Application.Config;
using Domain.Modules.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Services.StoreModule;
using Presentation.Globalization;
using Presentation.Models.ProductViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.StoreModule.AppCommands;

namespace Presentation.Controllers
{
    [AuthorizeSellerOrSellerAssistant]
    public class ProductAvailabilityController : BaseController
    {
        private MaaSConfig Config => MaaSConfig.Current;

        private const string ViewPathFmt = "~/Views/Products/ProductAvailability/{0}.cshtml";

        private readonly NewProductAvailabilityAppCommandHandler _newProductAvailabilityAppCommandHandler;
        private readonly EditProductAvailabilityAppCommandHandler _editProductAvailabilityAppCommandHandler;
        private readonly DeleteProductAvailabilityAppCommandHandler _deleteProductAvailabilityAppCommandHandler;
        private readonly ProductService _productService;
        private readonly ProductAvailabilityService _productAvailabilityService;

        public ProductAvailabilityController(
            NewProductAvailabilityAppCommandHandler newProductAvailabilityAppCommandHandler,
            EditProductAvailabilityAppCommandHandler editProductAvailabilityAppCommandHandler,
            DeleteProductAvailabilityAppCommandHandler deleteProductAvailabilityAppCommandHandler,
            ProductService productService,
            ProductAvailabilityService productAvailabilityService)
        {
            _newProductAvailabilityAppCommandHandler = newProductAvailabilityAppCommandHandler;
            _editProductAvailabilityAppCommandHandler = editProductAvailabilityAppCommandHandler;
            _deleteProductAvailabilityAppCommandHandler = deleteProductAvailabilityAppCommandHandler;
            _productService = productService;
            _productAvailabilityService = productAvailabilityService;
        }

        #region Availability

        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult NewAvailability(Guid productId)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled) return HttpNotFound();

            SetViewBagForAvailability();

            var product = _productService.GetById(productId);

            return View(string.Format(ViewPathFmt, "NewAvailability"), ProductAvailabilityVM.Prepare(product));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult NewAvailability(ProductAvailabilityVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetNewCommand(model);
                var commandResult = _newProductAvailabilityAppCommandHandler.Handle(command);

                if (commandResult.OK)
                    return RedirectToAction("EditProduct", "Store", new { id = model.ProductId, tab = "product-availability", lang = Request.GetCurrentLang(), area = "" });

                ModelState.AddErrors(commandResult);
            }

            model.WithProduct(_productService.GetById(model.ProductId));

            SetViewBagForAvailability();

            return View(string.Format(ViewPathFmt, "NewAvailability"), model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult EditAvailability(Guid id, Guid productId)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled) return HttpNotFound();

            SetViewBagForAvailability();

            var sellerId = GetPersonId();

            var availability = _productAvailabilityService.GetById(id, sellerId);

            if (availability == null) return HttpNotFound();

            return View(string.Format(ViewPathFmt, "EditAvailability"), ProductAvailabilityVM.Prepare(availability));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult EditAvailability(ProductAvailabilityVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetEditCommand(model);
                var commandResult = _editProductAvailabilityAppCommandHandler.Handle(command);

                if (commandResult.OK)
                    return RedirectToAction("EditProduct", "Store", new { id = model.ProductId, tab = "product-availability", lang = Request.GetCurrentLang(), area = "" });

                ModelState.AddErrors(commandResult);
            }

            SetViewBagForAvailability();

            model.WithProduct(_productService.GetById(model.ProductId));

            return View(string.Format(ViewPathFmt, "EditAvailability"), model);
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult Delete(Guid id, Guid productId)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled) return HttpNotFound();

            var command = new DeleteProductAvailabilityAppCommand(
                GetPersonId(), id);

            var result = _deleteProductAvailabilityAppCommandHandler.Handle(command);

            if (result.IsFailure) ModelState.AddErrors(result);

            this.SetSuccessMessage(Request.GetLocalizedString("PRODUCT_AVAILABILITY_DELETED_MESSAGE"), true);
            return RedirectToAction("EditProduct", "Store", new { id = productId, tab = "product-availability", lang = Request.GetCurrentLang() });
        }

        private NewProductAvailabilityAppCommand GetNewCommand(ProductAvailabilityVM model)
        {
            return NewProductAvailabilityAppCommand.New(
                GetPersonId(), model.ProductId,
                model.DayOfWeek, model.StartTime ?? TimeSpan.Zero, model.EndTime ?? TimeSpan.Zero,
                model.CurrentStock ?? 0, model.Discount ?? 0);
        }

        private EditProductAvailabilityAppCommand GetEditCommand(ProductAvailabilityVM model)
        {
            return EditProductAvailabilityAppCommand.New(
                model.Id, GetPersonId(),
                model.CurrentStock ?? 0, model.Discount ?? 0);
        }

        private void SetViewBagForAvailability()
        {
            ViewBag.DaysOfWeek = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Sunday, "SUNDAY_LABEL" },
                { DayOfWeek.Monday, "MONDAY_LABEL" },
                { DayOfWeek.Tuesday, "TUESDAY_LABEL" },
                { DayOfWeek.Wednesday, "WEDNESDAY_LABEL" },
                { DayOfWeek.Thursday, "THURSDAY_LABEL" },
                { DayOfWeek.Friday, "FRIDAY_LABEL" },
                { DayOfWeek.Saturday, "SATURDAY_LABEL" }
            };
        }

        #endregion Availability

        #region Recurring

        public ActionResult NewRecurring(Guid productId)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled) return HttpNotFound();

            SetViewBagForRecurring();

            var product = _productService.GetById(productId);

            if (!product.IsPackageType) return HttpNotFound();

            return View(string.Format(ViewPathFmt, "NewRecurring"), RecurringProductAvailabilityVM.Prepare(product));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRecurring(RecurringProductAvailabilityVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetNewCommand(model);
                var commandResult = _newProductAvailabilityAppCommandHandler.Handle(command);

                if (commandResult.OK)
                    return RedirectToAction("EditProduct", "Store", new { id = model.ProductId, tab = "product-availability", lang = Request.GetCurrentLang(), area = "" });

                ModelState.AddErrors(commandResult);
            }

            model.WithProduct(_productService.GetById(model.ProductId));

            SetViewBagForRecurring();

            return View(string.Format(ViewPathFmt, "NewRecurring"), model);
        }

        public ActionResult EditRecurring(Guid id, Guid productId)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled) return HttpNotFound();

            SetViewBagForRecurring();

            var sellerId = GetPersonId();

            var availability = _productAvailabilityService.GetById(id, sellerId);

            if (availability == null) return HttpNotFound();

            return View(string.Format(ViewPathFmt, "EditRecurring"), RecurringProductAvailabilityVM.Prepare(availability));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRecurring(RecurringProductAvailabilityVM model)
        {
            if (!Config.Product.BusinessModel.Rental.Enabled ||
                !Config.Product.BusinessModel.Rental.GridView.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Enabled ||
                !Config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled) return HttpNotFound();

            if (ModelState.IsValid)
            {
                var command = GetEditCommand(model);
                var commandResult = _editProductAvailabilityAppCommandHandler.Handle(command);

                if (commandResult.OK)
                    return RedirectToAction("EditProduct", "Store", new { id = model.ProductId, tab = "product-availability", lang = Request.GetCurrentLang(), area = "" });

                ModelState.AddErrors(commandResult);
            }

            SetViewBagForRecurring();

            model.WithProduct(_productService.GetById(model.ProductId));

            return View(string.Format(ViewPathFmt, "EditRecurring"), model);
        }

        private NewRecurringProductAvailabilityAppCommand GetNewCommand(RecurringProductAvailabilityVM model)
        {
            return NewRecurringProductAvailabilityAppCommand.New(
                GetPersonId(), model.ProductId,
                model.DaysOfWeek?.ToArray(), model.Unit, model.UnitValue,
                model.StartTime ?? TimeSpan.Zero, model.EndTime ?? TimeSpan.Zero,
                model.CurrentStock ?? 0, model.Discount ?? 0);
        }

        private EditRecurringProductAvailabilityAppCommand GetEditCommand(RecurringProductAvailabilityVM model)
        {
            return EditRecurringProductAvailabilityAppCommand.New(
                model.Id, GetPersonId(),
                model.CurrentStock ?? 0, model.Discount ?? 0);
        }

        private void SetViewBagForRecurring()
        {
            SetViewBagForAvailability();

            ViewBag.RecurringUnits = new Dictionary<RecurringProductAvailabilityUnit, string>
            {
                { RecurringProductAvailabilityUnit.Days, RecurringProductAvailabilityUnit.Days.GetDescriptionLocalized() },
                { RecurringProductAvailabilityUnit.Weeks, RecurringProductAvailabilityUnit.Weeks.GetDescriptionLocalized() }
            };
        }

        #endregion Recurring
    }
}