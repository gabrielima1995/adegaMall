using Application.Config;
using System.Net;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class AppStartController : Controller
    {
        public ActionResult WarmUp(string k)
        {
            if (k != "4a1b152f71f247c690204aecc35ac324")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var config = MaaSConfig.Current;

            if (config.Portfolio.Enabled)
            {

            }

            if (config.Product.Enabled)
            {

            }

            if (config.Quote.Enabled)
            {

            }

            if (config.Subscription.Enabled)
            {

            }

            return new HttpStatusCodeResult(204);
        }
    }
}