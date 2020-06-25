using System.Web.Mvc;
using V2.Application.Modules.Customization.Queries;

namespace Presentation.Controllers
{
    public class PlatformCustomizationController : Controller
    {
        private readonly IPlatformCustomizationQueries _platformCustomizationQueries;

        public PlatformCustomizationController(IPlatformCustomizationQueries platformCustomizationQueries)
        {
            _platformCustomizationQueries = platformCustomizationQueries;
        }

        public ContentResult Head()
        {
            return Content(_platformCustomizationQueries.GetHtmlCustomization()?.Head);
        }

        public ContentResult BodyStart()
        {
            return Content(_platformCustomizationQueries.GetHtmlCustomization()?.BodyStart);
        }

        public ContentResult BodyEnd()
        {
            return Content(_platformCustomizationQueries.GetHtmlCustomization()?.BodyEnd);
        }
    }
}