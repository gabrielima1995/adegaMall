using Application.Home;
using DevTrends.MvcDonutCaching;
using System;
using System.Web.Mvc;
using Application.Modules.Products;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeAppService homeAppService;
        private readonly CategoryMenuAppService categoryMenuAppService;

        public HomeController(
            HomeAppService homeAppService,
            CategoryMenuAppService categoryMenuAppService)
        {
            this.homeAppService = homeAppService;
            this.categoryMenuAppService = categoryMenuAppService;
        }

        [DonutOutputCache(CacheProfile = "Home")]
        public ActionResult Index()
        {
            var response = homeAppService.Prepare();

            if (TempData.ContainsKey("SearchError"))
            {
                ViewData.Add("SearchError", TempData["SearchError"]);
                TempData.Remove("SearchError");
            }

            return View(response);
        }

        public ActionResult TestError()
        {
            var ex = new Exception("This is a test exception.", new Exception("And this is the inner exception."));
            throw ex;
        }

        [DonutOutputCache(CacheProfile = "CategoryMenu")]
        public PartialViewResult MenuCategorias(bool mobile = false)
        {
            ViewBag.Mobile = mobile;

            var menu = categoryMenuAppService.Get();

            return PartialView("_MenuCategorias", menu);
        }

        [DonutOutputCache(CacheProfile = "CategoryMenu")]
        public PartialViewResult CardCategorias()
        {
            var menu = categoryMenuAppService.Get();

            return PartialView("_CardCategorias", menu);
        }
    }
}