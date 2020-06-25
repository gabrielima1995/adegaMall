using Application.Modules.Subscriptions;
using System.Web.Mvc;
using V2.Application.Modules.Subscriptions.Queries;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionQueries _queries;
        private readonly SubscriptionAppService _subscriptionAppService;

        public SubscriptionsController(
            ISubscriptionQueries queries,
            SubscriptionAppService subscriptionAppService)
        {
            _queries = queries;
            _subscriptionAppService = subscriptionAppService;
        }

        public ActionResult Index()
        {
            var list = _queries.ListSubscriptions();
            return View(list);
        }
    }
}