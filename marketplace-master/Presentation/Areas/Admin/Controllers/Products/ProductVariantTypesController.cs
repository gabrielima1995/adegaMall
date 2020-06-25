using Application.Modules.Products.Variants;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Products
{
    [AuthorizeAdmin]
    public class ProductVariantTypesController : Controller
    {
        private const string _viewPathFormat = "~/Areas/Admin/Views/Products/ProductVariantTypes/{0}.cshtml";

        private readonly IProductVariantTypeAppService _productVariantTypeAppService;

        public ProductVariantTypesController(
            IProductVariantTypeAppService productVariantTypeAppService)
        {
            _productVariantTypeAppService = productVariantTypeAppService;
        }

        public ActionResult Index()
        {
            return View(string.Format(_viewPathFormat, "Index"), _productVariantTypeAppService.GetList());
        }

        public ActionResult Create()
        {
            return View(string.Format(_viewPathFormat, "Create"), _productVariantTypeAppService.PrepareNewVariantType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductVariantTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productVariantTypeAppService.SaveVariantType(model);

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
            var model = _productVariantTypeAppService.GetVariantType(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductVariantTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _productVariantTypeAppService.SaveVariantType(model);

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
            _productVariantTypeAppService.DeleteVariantType(id);

            return RedirectToAction("Index");
        }
    }
}