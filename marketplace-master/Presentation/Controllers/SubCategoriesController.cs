using Application.Modules.Portfolio.SubCategories;
using Presentation.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly ISubCategoryAppService _subCategoryAppService;

        public SubCategoriesController(ISubCategoryAppService subCategoryAppService)
        {
            _subCategoryAppService = subCategoryAppService;
        }

        public ActionResult Select(int? category, string seller)
        {
            if (string.IsNullOrEmpty(seller))
                seller = null;

            var response = _subCategoryAppService.GetSelector(category, seller);
            var options = $"<option value>{Request.GetLocalizedString("SUB_CATEGORIES_SELECT")}</option>";

            options = response.SubCategories.Aggregate(options, (current, subCategory) => current + $"<option value=\"{subCategory.Id}\">{subCategory.Name}</option>");

            return Content(options, "text/html");
        }
    }
}