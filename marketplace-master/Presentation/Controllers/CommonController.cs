using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class CommonController : Controller
    {
        public PartialViewResult Header()
        {
            return PartialView("_Header");
        }

        public PartialViewResult Footer()
        {
            return PartialView("_Footer");
        }

        public PartialViewResult MenuMobile()
        {
            return PartialView("_MenuMobile");
        }
    }
}