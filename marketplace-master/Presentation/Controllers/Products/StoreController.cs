using Application.Config;
using Application.Helpers;
using Application.Modules.Products;
using Application.Modules.Products.Store;
using Application.Modules.Products.Variants;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.Products.Variants;
using Domain.Services.StoreModule;
using Microsoft.AspNet.Identity;
using Presentation.Globalization;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.ProductTemplateModule;
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.StoreModule.AppCommands;
using V2.Application.Modules.StoreModule.Queries;

namespace Presentation.Controllers.Products
{
    [AuthorizeSellerOrSellerAssistant]
    public class StoreController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Products/Store/{0}.cshtml";

        private readonly ProductCategoryService _productCategoryService;
        private readonly ProductVariantService _productVariantService;
        private readonly ProductVariantTypeService _productVariantTypeService;
        private readonly IStoreAppService _storeAppService;
        private readonly StoreService _storeService;
        private readonly ICustomFieldAppService _customFieldAppService;
        private readonly DeleteLockAppCommandHandler _deleteLockAppCommandHandler;
        private readonly IProductTemplateStoreQueries productTemplateQueries;
        private readonly IStoreProductQueries _storeProductQueries;

        public StoreController(
            ProductCategoryService productCategoryService,
            ProductVariantTypeService productVariantTypeService,
            ProductVariantService productVariantService,
            IStoreAppService storeAppService,
            StoreService storeService,
            ICustomFieldAppService customFieldAppService,
            DeleteLockAppCommandHandler deleteLockAppCommandHandler,
            IProductTemplateStoreQueries productTemplateQueries,
            IStoreProductQueries storeProductQueries)
        {
            _productCategoryService = productCategoryService;
            _productVariantService = productVariantService;
            _productVariantTypeService = productVariantTypeService;
            _storeAppService = storeAppService;
            _storeService = storeService;
            _customFieldAppService = customFieldAppService;
            _deleteLockAppCommandHandler = deleteLockAppCommandHandler;
            this.productTemplateQueries = productTemplateQueries;
            _storeProductQueries = storeProductQueries;
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.StoreEdit)]
        public ActionResult DefaultShipping()
        {
            var sellerId = GetPersonId();
            var store = _storeService.GetStore(sellerId);
            var model = new DefaultShippingViewModel(store);
            return View(string.Format(ViewPathFmt, "DefaultShipping"), model);
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.StoreEdit)]
        public ActionResult DefaultShipping(DefaultShippingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sellerId = GetPersonId();
                var store = _storeService.GetStore(sellerId);

                model.SetStoreValues(store);
                var result = _storeService.Save(store);

                if (result.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToAction("Index", "Store", new { lang = Request.GetCurrentLang() });
                }

                ModelState.AddErrors(result.Errors);
            }

            return View(string.Format(ViewPathFmt, "DefaultShipping"), model);
        }

        public ActionResult AddPrivateFleetParam(int range = 0)
        {
            return PartialView(string.Format(ViewPathFmt, "_ProductPrivateFleetParameter"), new PrivateFleetParameterViewModel
            {
                Range = range + 10
            });
        }

        public ActionResult AddSingleFreightParam(int range = 0)
        {
            return PartialView(string.Format(ViewPathFmt, "_SingleFreightParameter"), new SingleFreightParameterViewModel
            {
                Range = range + 10
            });
        }

        public ActionResult AddVariant()
        {
            var response = new ProductVariantTypeAndItemSelector();
            response.VariantTypes = _productVariantTypeService.SelectorList();
            response.Variants = new List<ProductVariant>();
            return PartialView(string.Format(ViewPathFmt, "_ProductVariant"), response);
        }

        public ActionResult AddStockVariant(int variantType)
        {
            var response = new ProductStockVariantItemViewModel();
            response.StockVariants = _productVariantService.SelectorList(variantType);
            return PartialView(string.Format(ViewPathFmt, "_ProductStockVariant"), response);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult CreateProduct(Guid? templateId)
        {
            if (MaaSConfig.Current.Product.Templates.Enabled)
            {
                if (MaaSConfig.Current.Product.Templates.ForceSellerToUseTemplates && templateId == null)
                    return RedirectToAction("Index");
            }
            else templateId = null;

            ViewBag.Title = Request.GetLocalizedString("ADD_PRODUCT_LINK");
            var model = _storeAppService.PrepareNewProduct(templateId);

            if (MaaSConfig.Current.CustomFields.Product.Enabled)
                _customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View(string.Format(ViewPathFmt, "Product"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult CreateProduct(ProductEditorViewModel model)
        {
            ViewBag.Title = Request.GetLocalizedString("ADD_PRODUCT_LINK");
            model.CurrentStock = model.UpdatedStock;
            return SaveProduct(model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult DeleteProduct(Guid id)
        {
            _storeAppService.DeleteProduct(id, GetPersonId());
            var config = MaaSConfig.Current;
            if (config.Product.BusinessModel.Rental.Enabled)
            {
                var command = new DeleteLockForProductAppComand()
                {
                    SellerId = GetPersonId(),
                    ProductId = id
                };
                var result = _deleteLockAppCommandHandler.Handle(command);
                if (result.IsFailure)
                    ModelState.AddErrors(result);
            }

            this.SetSuccessMessage(Request.GetLocalizedString("PRODUCT_DELETED_MESSAGE"), true);
            var action = MaaSConfig.Current.Product.Templates.Enabled && MaaSConfig.Current.Product.Templates.ForceSellerToUseTemplates ? "Templates" : "Index";
            return RedirectToAction(action, "Store", new { lang = Request.GetCurrentLang() });
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsView)]
        public ActionResult EditProduct(Guid id, string tab)
        {
            ViewBag.Title = Request.GetLocalizedString("EDIT_PRODUCT_LINK");
            ViewBag.Tab = tab;

            var model = _storeAppService.GetProduct(id, GetPersonId());

            if (model == null)
                return HttpNotFound();

            if (MaaSConfig.Current.CustomFields.Product.Enabled)
            {
                model.Id = id;
                _customFieldAppService.ReadCustomFieldsToViewModel(model);
            }

            model.UpdatedStock = model.CurrentStock;
            return View(string.Format(ViewPathFmt, "Product"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProductsEdit)]
        public ActionResult EditProduct(ProductEditorViewModel model)
        {
            ViewBag.Title = Request.GetLocalizedString("EDIT_PRODUCT_LINK");
            return SaveProduct(model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.StoreView + "," + SellerAssistantPermissions.ProductsView)]
        public ActionResult Index()
        {
            var sellerId = GetPersonId();
            var store = _storeService.GetStore(sellerId);
            var model = new StoreVM(store);
            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.StoreView + "," + SellerAssistantPermissions.ProductsView)]
        public ActionResult GetProductsForStore(int start = 0, int lengh = 25)
        {
            var query = Request.QueryString["search[value]"];
            var sellerId = GetPersonId();
            var store = _storeProductQueries.ListProducts(sellerId, query, start, lengh);

            foreach (var item in store.Items)
                item.CoverImageUrl = ImgHelper.Product(item.CoverImageUrl, 80);

            var json = JsonHelper.ToJson(store);
            return Content(json, "application/json");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.StoreEdit)]
        public ActionResult Index(StoreVM model)
        {
            if (ModelState.IsValid)
            {
                var store = model.ToDomainModel();
                var result = _storeService.Save(store);

                if (result.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToAction("Index", "Store", new { lang = Request.GetCurrentLang() });
                }

                ModelState.AddErrors(result.Errors);
            }

            var currentStore = _storeService.GetStore(model.SellerId);
            model = new StoreVM(currentStore);
            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.StoreView + "," + SellerAssistantPermissions.ProductsView)]
        public ActionResult Templates(string mode)
        {
            var sellerId = GetPersonId();
            var templates = productTemplateQueries.ListStoreTemplates(sellerId, 0, 1000);
            var model = new StoreTemplatesViewModel(templates, mode);
            return View(string.Format(ViewPathFmt, "Templates"), model);
        }

        [HttpPost]
        public ActionResult NewImagesHtml(List<string> images)
        {
            var list = images
                .Select(i => new ProductImage { Url = i })
                .ToList();
            return PartialView(string.Format(ViewPathFmt, "_ProductNewImages"), list);
        }

        private ActionResult SaveProduct(ProductEditorViewModel model)
        {
            model.SellerId = GetPersonId();

            if (ModelState.IsValid)
            {
                bool customFieldsAreValid = true;

                if (MaaSConfig.Current.CustomFields.Product.Enabled)
                {
                    _customFieldAppService.LoadFieldDefinitionsFor(model);
                    var validation = _customFieldAppService.ValidateModel(model);

                    if (validation.IsFailure)
                    {
                        ModelState.AddErrors(validation);
                        customFieldsAreValid = false;
                    }
                }

                if (customFieldsAreValid)
                {
                    var saveResult = _storeAppService.SaveProduct(model);

                    if (saveResult.OK)
                    {
                        if (MaaSConfig.Current.CustomFields.Product.Enabled && model.HasCustomFields())
                            _customFieldAppService.StoreCustomFields(model);

                        this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);

                        return model.TemplateId.HasValue
                            ? RedirectToAction("Templates", "Store", new { mode = Url.IsCurrent("CreateProduct", "Store") ? "available" : "mine" })
                            : RedirectToAction("Index", "Store");
                    }

                    this.AddErrors(saveResult);
                }
            }

            _storeAppService.PrepareProduct(model);

            if (MaaSConfig.Current.CustomFields.Product.Enabled)
                _customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View(string.Format(ViewPathFmt, "Product"), model);
        }
    }
}