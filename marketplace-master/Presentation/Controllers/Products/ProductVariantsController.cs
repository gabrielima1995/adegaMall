using Application.Modules.Products.Variants;
using Presentation.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Controllers.Products
{
    public class ProductVariantsController : Controller
    {
        private readonly IProductVariantAppService _productVariantAppService;

        public ProductVariantsController(IProductVariantAppService productVariantAppService)
        {
            _productVariantAppService = productVariantAppService;
        }

        public ActionResult Select(int? variantType)
        {
            var response = _productVariantAppService.GetSelector(variantType);

            var options = $"<option value>{Request.GetLocalizedString("PRODUCT_VARIANT_SELECT")}</option>";

            options = response.ProductVariants.Aggregate(options, (current, variant) => current + $"<option value=\"{variant.Id}\">{variant.Name}</option>");

            return Content(options, "text/html");
        }
    }
}