using Application.Modules.Products.Variants;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Products
{
    [AuthorizeAdmin]
    public class ProductVariantsController : Controller
    {
        private const string _viewPathFormat = "~/Areas/Admin/Views/Products/ProductVariants/{0}.cshtml";

        private readonly IProductVariantAppService _productVariantAppService;

        public ProductVariantsController(
            IProductVariantAppService productVariantAppService)
        {
            _productVariantAppService = productVariantAppService;
        }

        public ActionResult Index()
        {
            return View(string.Format(_viewPathFormat, "Index"), _productVariantAppService.GetList());
        }

        public ActionResult Create()
        {
            return View(string.Format(_viewPathFormat, "Create"), _productVariantAppService.PrepareNewProductVariant());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductVariantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productVariantAppService.SaveProductVariant(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            _productVariantAppService.PrepareProductVariant(model);

            return View(string.Format(_viewPathFormat, "Create"), model);
        }

        public ActionResult Edit(int id)
        {
            var model = _productVariantAppService.GetVariant(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductVariantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productVariantAppService.SaveProductVariant(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            _productVariantAppService.PrepareProductVariant(model);

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _productVariantAppService.DeleteProductVariant(id);

            return RedirectToAction("Index");
        }
    }
}