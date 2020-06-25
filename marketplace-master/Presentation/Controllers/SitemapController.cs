using System.Text;
using System.Web.Mvc;
using V2.Application.Modules.SiteMap;

namespace Presentation.Controllers
{
    public class SitemapController : Controller
    {
        private readonly SiteMapAppService _siteMapAppService;

        public SitemapController(SiteMapAppService siteMapAppService)
        {
            _siteMapAppService = siteMapAppService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var xml = _siteMapAppService.GenerateSiteMapXml();

            return Content(xml, "application/xml", Encoding.UTF8);
        }
    }
}