using Application.StaticPages;
using Presentation.Globalization;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class HelpMeToFindController : Controller
    {
        private readonly IStaticPagesAppService _staticPagesAppService;

        public HelpMeToFindController(
            IStaticPagesAppService staticPagesAppService)
        {
            _staticPagesAppService = staticPagesAppService;
        }

        public ActionResult Index()
        {
            ViewBag.Content = _staticPagesAppService.HelpMeToFind(Request.GetCurrentLang());

            return View();
        }
    }
}