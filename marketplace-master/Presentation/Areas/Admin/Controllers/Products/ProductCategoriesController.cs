using Application.Modules.Products.Categories;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Products
{
    [AuthorizeAdmin]
    public class ProductCategoriesController : Controller
    {
        private const string _viewPathFormat = "~/Areas/Admin/Views/Products/ProductCategories/{0}.cshtml";

        private readonly IProductCategoryAppService _productCategoryAppService;

        public ProductCategoriesController(
            IProductCategoryAppService productCategoryAppService)
        {
            _productCategoryAppService = productCategoryAppService;
        }

        public ActionResult Index()
        {
            return View(string.Format(_viewPathFormat, "Index"), _productCategoryAppService.GetList());
        }

        public ActionResult Create()
        {
            return View(string.Format(_viewPathFormat, "Create"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productCategoryAppService.SaveCategory(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            return View(string.Format(_viewPathFormat, "Create"), model);
        }

        public ActionResult Edit(int id)
        {
            var model = _productCategoryAppService.GetCategory(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productCategoryAppService.SaveCategory(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _productCategoryAppService.DeleteCategory(id);

            return RedirectToAction("Index");
        }
    }
}