using Application.Home;
using DevTrends.MvcDonutCaching;
using System;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeAppService homeAppService;

        public HomeController(HomeAppService homeAppService)
        {
            this.homeAppService = homeAppService;
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
    }
}