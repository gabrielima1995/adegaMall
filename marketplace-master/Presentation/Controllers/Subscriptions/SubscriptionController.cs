using Application.Modules.Subscriptions;
using Presentation.Globalization;
using System.Web.Mvc;

namespace Presentation.Controllers.Subscriptions
{
    [AuthorizeSeller]
    public class SubscriptionController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Subscriptions/{0}.cshtml";

        private readonly SubscriptionAppService _subscriptionAppService;

        public SubscriptionController(SubscriptionAppService subscriptionAppService)
        {
            _subscriptionAppService = subscriptionAppService;
        }

        public ActionResult Index()
        {
            var sellerId = GetPersonId();
            var model = _subscriptionAppService.GetSubscription(sellerId);
            if (model != null) return View(string.Format(ViewPathFmt, "Index"), model);
            return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unsubscribe()
        {
            var sellerId = GetPersonId();

            if (ModelState.IsValid)
            {
                var validation = _subscriptionAppService.Unsubscribe(sellerId);

                if (validation.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("UNSUBSCRIBE_SUCCESS_MESSAGE"), true);
                    return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang() });
                }

                this.AddErrors(validation, true);
            }

            return Index();
        }
    }
}