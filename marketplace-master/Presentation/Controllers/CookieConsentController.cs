using System.Net;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [AllowAnonymous]
    public class CookieConsentController : Controller
    {
        public ActionResult Form()
        {
            return PartialView("_CookieConsentMessage");
        }

        public ActionResult AllowCookies(string returnUrl)
        {
            CookieConsent.SetCookieConsent(Response, true);
            return Redirect(returnUrl);
        }

        public ActionResult NoCookies(string returnUrl)
        {
            CookieConsent.SetCookieConsent(Response, false);

            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.OK);

            else return Redirect(returnUrl);
        }
    }
}