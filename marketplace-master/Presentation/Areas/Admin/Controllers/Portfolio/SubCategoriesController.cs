using Application.Modules.Portfolio.SubCategories;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Portfolio
{
    [AuthorizeAdmin]
    public class SubCategoriesController : Controller
    {
        private const string _viewPathFormat = "~/Areas/Admin/Views/Portfolio/SubCategories/{0}.cshtml";

        private readonly ISubCategoryAppService _subCategoryAppService;

        public SubCategoriesController(
            ISubCategoryAppService subCategoryAppService)
        {
            _subCategoryAppService = subCategoryAppService;
        }

        public ActionResult Index()
        {
            return View(string.Format(_viewPathFormat, "Index"), _subCategoryAppService.GetList().Where(x => !x.CategoryDeleted));
        }

        public ActionResult Create()
        {
            return View(string.Format(_viewPathFormat, "Create"), _subCategoryAppService.PrepareNewSubCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _subCategoryAppService.SaveSubCategory(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            _subCategoryAppService.PrepareSubCategory(model);

            return View(string.Format(_viewPathFormat, "Create"), model);
        }

        public ActionResult Edit(int id)
        {
            var model = _subCategoryAppService.GetSubCategory(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _subCategoryAppService.SaveSubCategory(model);

                if (result.OK)
                {
                    return RedirectToAction("Index");
                }

                this.AddErrors(result);
            }

            _subCategoryAppService.PrepareSubCategory(model);

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _subCategoryAppService.DeleteSubCategory(id);

            return RedirectToAction("Index");
        }
    }
}