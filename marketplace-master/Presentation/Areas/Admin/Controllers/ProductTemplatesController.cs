using Domain.Modules.Products.Variants;
using Domain.Modules.ProductTemplateModule;
using Domain.Services.StoreModule;
using Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.ProductTemplateModule;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class ProductTemplatesController : Controller
    {
        private readonly IProductTemplateAdminQueries queries;
        private readonly IProductTemplateRepository repository;
        private readonly ProductCategoryService categoryService;
        private readonly ProductVariantTypeService variantService;

        public ProductTemplatesController(
            IProductTemplateAdminQueries queries,
            IProductTemplateRepository repository,
            ProductCategoryService categoryService,
            ProductVariantTypeService variantService)
        {
            this.queries = queries;
            this.repository = repository;
            this.categoryService = categoryService;
            this.variantService = variantService;
        }

        public ActionResult Index()
        {
            var items = queries.ListProductTemplateItems();
            var model = new ProductTemplateIndexViewModel
            {
                Items = items
            };
            return View("~/Areas/Admin/Views/ProductTemplates/Index.cshtml", model);
        }

        public ActionResult Create()
        {
            var model = new ProductTemplateCreateEditViewModel();
            SetViewModelSelectOptions(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductTemplateCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = ProductTemplate.Create();
                model.CopyTo(template);

                var result = repository.Add(template);
                if (result.OK) return RedirectToAction("Index");

                ModelState.AddErrors(result);
            }

            SetViewModelSelectOptions(model);
            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var template = repository.Find(id);
            if (template == null) return RedirectToAction("Index");

            var model = new ProductTemplateDetailsViewModel(template);

            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var template = repository.Find(id);
            if (template == null) return RedirectToAction("Index");

            var model = new ProductTemplateCreateEditViewModel(template);
            SetViewModelSelectOptions(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProductTemplateCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = repository.Find(model.Id);
                if (template == null) return RedirectToAction("Index");

                model.CopyTo(template);

                var result = repository.Update(template);
                if (result.OK) return RedirectToAction("Index");

                ModelState.AddErrors(result);
            }

            SetViewModelSelectOptions(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var template = repository.Find(id);
            if (template != null)
                repository.Remove(template);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ImagesHtml(IEnumerable<string> images)
        {
            var model = images.Select(url => new ProductTemplateImageViewModel(url));
            return PartialView("_Images", model);
        }

        private void SetViewModelSelectOptions(ProductTemplateCreateEditViewModel model)
        {
            var categories = categoryService.ListActive();
            model.SetCategoryOptions(categories);

            var variants = variantService.ListVariantTypesAndItems();
            model.SetVariantOptions(variants);
        }
    }
}