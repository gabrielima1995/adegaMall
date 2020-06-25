using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Domain.Services.StoreModule;
using Presentation.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class EcommerceHomeController : Controller
    {
        private readonly EcommerceHomeService _ecommerceHomeService;
        private readonly ProductCategoryService _productCategoryService;
        private readonly ICustomItemRepository _customItemRepository;
        private readonly ICustomItemCategoryRepository _customItemCategoryRepository;

        public EcommerceHomeController(
            EcommerceHomeService ecommerceHomeService,
            ProductCategoryService productCategoryService,
            ICustomItemRepository customItemRepository,
            ICustomItemCategoryRepository customItemCategoryRepository)
        {
            _ecommerceHomeService = ecommerceHomeService;
            _productCategoryService = productCategoryService;
            _customItemRepository = customItemRepository;
            _customItemCategoryRepository = customItemCategoryRepository;
        }

        public ActionResult Index()
        {
            var home = _ecommerceHomeService.GetOrNew();
            var viewModel = new EcommerceHomeVM(home);
            viewModel.CustomItems = _customItemRepository.ListAll();
            viewModel.CustomItems.OrderBy(x => x.CategoryId.ToString());
            return View(viewModel);
        }

        public ActionResult CategoryModal(Guid? categoryId)
        {
            EcommerceHomeCategoryVM viewModel = null;

            if (categoryId == null)
            {
                var home = _ecommerceHomeService.GetOrNew();

                viewModel = new EcommerceHomeCategoryVM
                {
                    EcommerceHomeId = home.Id,
                    IsEdit = false
                };
            }
            else
            {
                var category = _ecommerceHomeService.GetHomeCategory(categoryId.Value);
                viewModel = new EcommerceHomeCategoryVM(category) { IsEdit = true };
            }

            SetCategoryModalViewBag(viewModel);

            return PartialView("_CategoryModalPartial", viewModel);
        }

        public ActionResult SaveCategory(EcommerceHomeCategoryVM viewModel)
        {
            bool redirect = false;

            if (ModelState.IsValid)
            {
                redirect = viewModel.UseCustomCategory;

                if (viewModel.IsEdit)
                {
                    var domainModel = _ecommerceHomeService.GetHomeCategory(viewModel.Id);
                    viewModel.UpdateDomainModel(domainModel);

                    if (viewModel.UseCustomCategory)
                    {
                        var customCategory = _customItemCategoryRepository.GetById(viewModel.CustomCategoryId);
                        customCategory.Name = viewModel.CustomCategoryName;
                        customCategory.CarouselDisplay = viewModel.CarouselDisplay.GetValueOrDefault();
                        var res = _customItemCategoryRepository.Update(customCategory, true);
                    }

                    var updateResult = _ecommerceHomeService.UpdateHomeCategory(domainModel);
                    if (updateResult.OK)
                    {
                        if (!redirect)
                            return JavaScript("location.reload(true)");
                    }
                    else ModelState.AddErrors(updateResult);
                }
                else
                {
                    if (viewModel.UseCustomCategory && !string.IsNullOrWhiteSpace(viewModel.CustomCategoryName) && viewModel.CustomCategoryId == null)
                    {
                        var newCategory = new CustomItemCategory()
                        {
                            Name = viewModel.CustomCategoryName,
                            CarouselDisplay = viewModel.CarouselDisplay.GetValueOrDefault(),
                        };

                        var result = _customItemCategoryRepository.Add(newCategory, true);

                        if (!result.OK)
                            throw new Exception(result.Errors.ToSimpleString());

                        viewModel.CustomCategoryId = newCategory.Id;
                    }

                    var domainModel = new EcommerceHomeCategory(
                        new EcommerceHome { Id = viewModel.EcommerceHomeId },
                        viewModel.ProductCategoryId.HasValue ? new ProductCategory { Id = viewModel.ProductCategoryId.Value } : null,
                        viewModel.Order)
                    { CustomItemCategoryId = viewModel.CustomCategoryId };



                    var addResult = _ecommerceHomeService.AddHomeCategory(domainModel);
                    if (addResult.OK)
                    {
                        if (!redirect)
                            return JavaScript("location.reload(true)");
                    }
                    else ModelState.AddErrors(addResult);
                }
            }

            if (redirect)
                return RedirectToAction("Index", "CustomItem", new { area = "Admin", catId = viewModel.CustomCategoryId.Value });

            SetCategoryModalViewBag(viewModel);

            return PartialView("_CategoryModalPartial", viewModel);
        }

        [HttpPost]
        public ActionResult RemoveCategory(Guid id)
        {
            var category = _ecommerceHomeService.GetHomeCategory(id);
            var result = _ecommerceHomeService.RemoveHomeCategory(category);
            return RedirectToAction("Index");
        }

        private void SetCategoryModalViewBag(EcommerceHomeCategoryVM viewModel)
        {
            var productCategories = _productCategoryService.ListActive();
            ViewBag.ProductCategoryId = new SelectList(productCategories, "Id", "Name", viewModel.ProductCategoryId);
        }
    }
}