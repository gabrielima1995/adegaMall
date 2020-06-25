using Application.Modules.Portfolio.Categories;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Portfolio
{
    [AuthorizeAdmin]
    public class CategoriesController : Controller
    {
        private const string _viewPathFormat = "~/Areas/Admin/Views/Portfolio/Categories/{0}.cshtml";

        private readonly ICategoryAppService _categoryAppService;

        public CategoriesController(
            ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }

        public ActionResult Index()
        {
            return View(string.Format(_viewPathFormat, "Index"), _categoryAppService.GetList());
        }

        public ActionResult Create()
        {
            return View(string.Format(_viewPathFormat, "Create"), _categoryAppService.PrepareNewCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryAppService.SaveCategory(model);

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
            var model = _categoryAppService.GetCategory(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(string.Format(_viewPathFormat, "Edit"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryAppService.SaveCategory(model);

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
            _categoryAppService.DeleteCategory(id);

            return RedirectToAction("Index");
        }
    }
}