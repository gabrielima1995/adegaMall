using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class PushApiController : Controller
    {
        private readonly IPushSubscriptionRepository _subscriptionRepository;

        public PushApiController(IPushSubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpPost]
        public ActionResult Subscribe(PushTokenContainerVM container)
        {
            var token = container?.Token;

            if (string.IsNullOrWhiteSpace(token))
                return Content("ERROR");

            var profile = User.Identity.GetLoginProfile();

            if (!profile.IsBuyerOrSeller())
                return Content("OK");

            var isSubscribed = _subscriptionRepository.GetByToken(token, profile.PersonId) != null;
            if (!isSubscribed)
            {
                var subscription = new MarketplacePushSubscription
                {
                    Token = token,
                    Id = Guid.NewGuid(),
                    UserId = profile.UserId,
                    UserRole = profile.IsSeller() ? "Seller" : "Buyer"
                };

                _subscriptionRepository.AddSubscription(subscription);
            }

            Session["NotificationToken"] = token;
            return Content("OK");
        }

        [HttpDelete]
        public ActionResult Unsubscribe([System.Web.Http.FromBody]string token)
        {
            var result = _subscriptionRepository.RemoveSubscription(token);

            if (result.OK)
                return Content("OK");

            return Content("FAIL");
        }
    }

    public class PushTokenContainerVM
    {
        public string Token { get; set; }
    }
}