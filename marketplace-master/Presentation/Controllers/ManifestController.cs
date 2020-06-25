using Application.Config;
using Newtonsoft.Json;
using Presentation.Models;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class ManifestController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new WebAppManifestVM(MaaSConfig.Current);
            var str = JsonConvert.SerializeObject(model);

            return Content(str, "application/json");
        }
    }
}