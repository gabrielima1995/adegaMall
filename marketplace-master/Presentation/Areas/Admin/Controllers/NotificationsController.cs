using Domain.Interfaces.Repositories;
using Presentation.Models;
using System.Web.Mvc;
using V2.Application.Modules.Admin;
using V2.Application.Modules.PushNotifications;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class NotificationsController : Controller
    {
        private readonly IPushNotificationService _notificationService;
        private readonly IPushSubscriptionRepository _subscriptionRepository;
        private readonly IAdminNotificationQueries _adminNotificationQueries;

        public NotificationsController(IPushNotificationService notificationService, IPushSubscriptionRepository subscriptionRepository, IAdminNotificationQueries adminNotificationQueries)
        {
            _notificationService = notificationService;
            _subscriptionRepository = subscriptionRepository;
            _adminNotificationQueries = adminNotificationQueries;
        }

        [HttpGet]
        public ActionResult Index(string status = null)
        {
            if (status != null && status == "success")
            {
                ViewBag.Success = true;
            }

            PushNotificationViewModel model = new PushNotificationViewModel();

            ViewBag.AllSellerCount = _adminNotificationQueries.GetDataForNotification(NotificationType.AllSellers).Count;
            ViewBag.EnabledSellerCount = _adminNotificationQueries.GetDataForNotification(NotificationType.EnabledSellers).Count;
            ViewBag.RequiredModerationSellersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.SellersPendingModeration).Count;

            ViewBag.AllBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.AllBuyers).Count;
            ViewBag.EnabledBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.EnabledBuyers).Count;
            ViewBag.RequiredModerationBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.BuyerPendingModeration).Count;

            return View(model);
        }

        [HttpPost]
        public ActionResult NotifyUsers(PushNotificationViewModel model)
        {
            if (model.TargetRole != NotificationType.AllBuyers && 
                model.TargetRole != NotificationType.EnabledBuyers &&
                model.TargetRole != NotificationType.BuyerPendingModeration &&
                model.TargetRole != NotificationType.AllSellers && 
                model.TargetRole != NotificationType.EnabledSellers && 
                model.TargetRole != NotificationType.SellersPendingModeration)
                ModelState.AddModelError("Error", "Invalid role selected");

            if (!string.IsNullOrWhiteSpace(model.Link) && !model.Link.StartsWith("https://"))
                ModelState.AddModelError("Error", "Informe um link HTTPS válido");

            if (ModelState.IsValid)
            {
                _notificationService.NotifyBybyRoleUserAndSeller(model.TargetRole.ToString(), new PushNotification()
                {
                    Body = model.Body,
                    Link = model.Link,
                    Title = model.Title,
                });

                return RedirectToAction("Index", new { status = "success" });
            }

            ViewBag.AllSellerCount = _adminNotificationQueries.GetDataForNotification(NotificationType.AllSellers).Count;
            ViewBag.EnabledSellerCount = _adminNotificationQueries.GetDataForNotification(NotificationType.EnabledSellers).Count;
            ViewBag.RequiredModerationSellersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.SellersPendingModeration).Count;

            ViewBag.AllBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.AllBuyers).Count;
            ViewBag.EnabledBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.EnabledBuyers).Count;
            ViewBag.RequiredModerationBuyersCount = _adminNotificationQueries.GetDataForNotification(NotificationType.BuyerPendingModeration).Count;


            return View(model);
        }
    }
}