using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class MetricsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}