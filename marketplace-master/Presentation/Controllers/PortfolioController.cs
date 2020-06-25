using Application.Config;
using Application.Modules.Portfolio;
using Application.Modules.Portfolio.SubCategories;
using Application.Sellers;
using Domain.Categories;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Modules.Portfolio;
using Domain.SubCategories;
using Presentation.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Controllers
{
    [AuthorizeSellerOrSellerAssistant]
    public class PortfolioController : BaseController
    {
        private readonly CategoryService_Obsolete _categoryService;
        private readonly ICustomFieldAppService _customFieldAppService;
        private readonly SellerAppService _sellerAppService;
        private readonly SubCategoryService_Obsolete _subCategoryService;

        public PortfolioController(
            CategoryService_Obsolete categoryService,
            SubCategoryService_Obsolete subCategoryService,
            SellerAppService sellerAppService,
            ICustomFieldAppService customFieldAppService)
        {
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _sellerAppService = sellerAppService;
            _customFieldAppService = customFieldAppService;
        }

        public ActionResult AddSubCategory()
        {
            var response = new SubCategoryAndCategorySelector
            {
                Categories = _categoryService.SelectorList(),
                SubCategories = new List<SubCategory>()
            };

            return PartialView("_PortfolioSubCategory", response);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult CreateItem()
        {
            ViewBag.Title = Request.GetLocalizedString("ADD_PORTFOLIO_ITEM_LINK");

            var model = _sellerAppService.PrepareNewPortfolioItem(GetPersonId());

            SetPortfolioItemCategories(model);

            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                _customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View("PortfolioItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult CreateItem(PortfolioItemViewModel model)
        {
            ViewBag.Title = Request.GetLocalizedString("ADD_PORTFOLIO_ITEM_LINK");

            return SavePortfolioItem(model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult DeleteItem(Guid id)
        {
            _sellerAppService.DeletePortfolioItem(id, GetPersonId());

            this.SetSuccessMessage(Request.GetLocalizedString("ITEM_DELETED_MESSAGE"), true);

            return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult EditItem(Guid id)
        {
            ViewBag.Title = Request.GetLocalizedString("EDIT_PORTFOLIO_ITEM_LINK");

            var model = _sellerAppService.GetPortfolioItem(id, GetPersonId());

            if (model == null)
                return HttpNotFound();

            SetPortfolioItemCategories(model);

            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                _customFieldAppService.ReadCustomFieldsToViewModel(model);

            return View("PortfolioItem", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult EditItem(PortfolioItemViewModel model)
        {
            ViewBag.Title = Request.GetLocalizedString("EDIT_PORTFOLIO_ITEM_LINK");
            return SavePortfolioItem(model);
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioView)]
        public ActionResult Index()
        {
            var response = _sellerAppService.GetPortfolio(GetPersonId());
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.PortfolioEdit)]
        public ActionResult Index(PortfolioRequestViewModel model)
        {
            model.SellerId = GetPersonId();

            if (ModelState.IsValid)
            {
                _sellerAppService.SavePortfolio(model);

                this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);

                return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
            }

            var response = _sellerAppService.GetPortfolio(model.SellerId, model.SubCategories);

            return View(response);
        }

        [HttpPost]
        public ActionResult NewCustomThumbHtml(string image)
        {
            return PartialView("_PortfolioItemThumbForm", image);
        }

        [HttpPost]
        public ActionResult NewImagesHtml(List<string> images)
        {
            var list = images.Select(i => new PortfolioItemImage { Url = i }).ToList();
            return PartialView("_PortfolioItemNewImages", list);
        }

        private ActionResult SavePortfolioItem(PortfolioItemViewModel model)
        {
            model.SellerId = GetPersonId();

            if (ModelState.IsValid)
            {
                if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                {
                    _customFieldAppService.LoadFieldDefinitionsFor(model);
                    var result = _customFieldAppService.ValidateModel(model);

                    if (!result.OK)
                        ModelState.AddErrors(result, true);
                }

                if (ModelState.IsValid)
                {
                    var validation = _sellerAppService.SavePortfolioItem(model);

                    if (validation.OK)
                    {
                        model.Id = validation.Result.Id;

                        if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                            if (model.HasCustomFields())
                                _customFieldAppService.StoreCustomFields(model);

                        this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                        return RedirectToAction("Index", "Portfolio", new { lang = Request.GetCurrentLang() });
                    }

                    this.AddErrors(validation);
                }
            }

            if (MaaSConfig.Current.CustomFields.Portfolio.Enabled)
                _customFieldAppService.ReadCustomFieldsToViewModel(model);

            SetPortfolioItemCategories(model);

            return View("PortfolioItem", model);
        }

        private void SetPortfolioItemCategories(PortfolioItemViewModel model)
        {
            var categoriesList = _categoryService.SelectorList();
            var subCategoriesList = _subCategoryService.SelectorList();

            if (model.SubCategories.Count == 0)
            {
                model.SubCategories.Add(new SubCategoryAndCategorySelector
                {
                    Category = 0,
                    SubCategory = 0,
                    Categories = categoriesList,
                    SubCategories = new List<SubCategory>()
                });
            }
            else
            {
                foreach (var sub in model.SubCategories)
                {
                    sub.Categories = categoriesList;
                    sub.SubCategories = subCategoriesList
                        .Where(x => x.Category.Id == (sub.Category ?? 0))
                        .ToList();
                }
            }
        }
    }
}