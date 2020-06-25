using Application.StaticPages;
using DevTrends.MvcDonutCaching;
using Presentation.Globalization;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [DonutOutputCache(CacheProfile = "General")]
    public class PrivacyPolicyController : Controller
    {
        private readonly IStaticPagesAppService _staticPagesAppService;

        public PrivacyPolicyController(
            IStaticPagesAppService staticPagesAppService)
        {
            _staticPagesAppService = staticPagesAppService;
        }

        public ActionResult Index()
        {
            ViewBag.Content = _staticPagesAppService.PrivacyPolicy(Request.GetCurrentLang());
            return View();
        }
    }
}