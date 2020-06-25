using Domain.Interfaces.Repositories;
using Presentation.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    public class CustomItemController : Controller
    {
        private readonly ICustomItemRepository customItemRepository;
        private readonly ICustomItemCategoryRepository customItemCategoryRepository;
        private readonly IEcommerceHomeCategoryRepository ecommerceHomeCategoryRepository;

        public CustomItemController(
            ICustomItemRepository customItemRepository,
            ICustomItemCategoryRepository customItemCategoryRepository,
            IEcommerceHomeCategoryRepository ecommerceHomeCategoryRepository)
        {
            this.customItemCategoryRepository = customItemCategoryRepository;
            this.customItemRepository = customItemRepository;
            this.ecommerceHomeCategoryRepository = ecommerceHomeCategoryRepository;
        }

        public ActionResult Index(Guid catId)
        {
            if (catId == null)
                return HttpNotFound();

            var itemCategory = customItemCategoryRepository.GetById(catId);

            if (itemCategory == null)
                return HttpNotFound();

            var homeCategory = ecommerceHomeCategoryRepository.Get(x => x.CustomItemCategoryId == itemCategory.Id);
            var homeCategoryVM = new EcommerceHomeCategoryVM(homeCategory);

            homeCategoryVM.CustomItems = customItemRepository.ListAll()
                .Where(x => x.CategoryId == homeCategoryVM.CustomCategoryId)
                .Select(x => new CustomItemViewModel(x)).ToList();

            return View("Index", homeCategoryVM);
        }

        [HttpPost]
        public ActionResult DeleteCustomItem(Guid id)
        {
            var item = customItemRepository.GetById(id);
            var catId = item.CategoryId;
            customItemRepository.Delete(item, true);
            return RedirectToAction("Index", new { catId });
        }

        [HttpPost]
        public ActionResult CustomItemOperation(CustomItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(entry => entry.Value.Errors.Count > 0).SelectMany(entry => entry.Value.Errors.Select(error => error.ErrorMessage));
                HttpContext.Items["errors"] = errors;
                return Index(model.CategoryId);
            }

            Framework.OperationResult result;

            if (model.IsEditing)
            {
                var customItem = customItemRepository.GetById(model.Id);

                customItem.ImageUrl = model.ImageUrl;
                customItem.TargetUrl = model.TargetUrl;

                customItem.StartDate = model.StartDate;
                customItem.EndDate = model.EndDate;

                customItem.CategoryId = model.CategoryId;
                customItem.Title = model.Title;

                if (customItem.EndDate.GetValueOrDefault().Year < 1900)
                    customItem.EndDate = null;

                if (customItem.StartDate.GetValueOrDefault().Year < 1900)
                    customItem.StartDate = null;

                result = customItemRepository.Update(customItem, true);
            }
            else
            {
                var item = model.ToEntity();
                result = customItemRepository.Add(item, true);
            }

            return RedirectToAction("Index", new { catId = model.CategoryId });
        }

        public ActionResult GetCustomItemForm(Guid? id, bool edit)
        {
            CustomItemViewModel viewModel;

            if (edit)
            {
                viewModel = new CustomItemViewModel(customItemRepository.GetById(id));
                viewModel.IsEditing = true;
            }
            else
                viewModel = new CustomItemViewModel();

            var view = PartialView("_CustomItemFormPartial", viewModel);
            view.ViewBag.Title = edit ? "Editar Item" : "Novo Item";

            return view;
        }
    }
}