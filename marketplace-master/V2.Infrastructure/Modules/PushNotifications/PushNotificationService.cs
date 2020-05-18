using Application.Config;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using V2.Application.Modules.Admin;
using V2.Application.Modules.PushNotifications;

namespace V2.Infrastructure.Modules.PushNotifications
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IPushSubscriptionRepository _subscriptionRepository;
        private readonly IAdminNotificationQueries _adminNotificationQueries;

        public PushNotificationService(IPushSubscriptionRepository subscriptionRepository, IAdminNotificationQueries adminNotificationQueries)
        {
            _subscriptionRepository = subscriptionRepository;
            _adminNotificationQueries = adminNotificationQueries;
        }

        public void NotifyByRole(string role, PushNotification message)
        {
            if (!MaaSConfig.Current.WebApp.PushNotifications.IsEnabled)
                return;

            var subs = _subscriptionRepository.ListByRoles(role).DistinctBy(x => x.Token);
            if (subs == null || !subs.Any())
                return;

            try
            {
                RestClient client = new RestClient(@"https://fcm.googleapis.com/fcm/send");
                client.AddDefaultHeader("Authorization", "key=" + MaaSConfig.Current.WebApp.PushNotifications.ServerKey);

                Parallel.ForEach(subs, x =>
                {
                    RestRequest request = GetRestRequest(x, message);
                    client.Post(request);
                });
            }
            catch
            {
            }
        }

        public void NotifyBybyRoleUserAndSeller(string role, PushNotification message)
        {
            if (!MaaSConfig.Current.WebApp.PushNotifications.IsEnabled)
                return;

            Enum.TryParse(role, out NotificationType notificationType);
            var subs = _adminNotificationQueries.GetDataForNotification(notificationType);
            if (subs == null || !subs.Any())
                return;

            try
            {
                RestClient client = new RestClient(@"https://fcm.googleapis.com/fcm/send");
                client.AddDefaultHeader("Authorization", "key=" + MaaSConfig.Current.WebApp.PushNotifications.ServerKey);

                Parallel.ForEach(subs, x =>
                {
                    RestRequest request = GetRestRequest(x, message);
                    client.Post(request);
                });
            }
            catch
            {
            }
        }

        public void NotifyUser(string userId, PushNotification message)
        {
            if (!MaaSConfig.Current.WebApp.PushNotifications.IsEnabled)
                return;

            var userSubs = _subscriptionRepository.ListByUserId(userId).DistinctBy(x => x.Token);

            if (!userSubs.Any())
                return;

            try
            {
                RestClient client = new RestClient(@"https://fcm.googleapis.com/fcm/send");
                client.AddDefaultHeader("Authorization", "key=" + MaaSConfig.Current.WebApp.PushNotifications.ServerKey);

                Parallel.ForEach(userSubs, x =>
                {
                    RestRequest request = GetRestRequest(x, message);
                    client.Post(request);
                });
            }
            catch
            {
            }
        }

        private RestRequest GetRestRequest(MarketplacePushSubscription subscription, PushNotification message)
        {
            RestRequest request = new RestRequest();
            var obj = new
            {
                notification = new
                {
                    title = message.Title,
                    body = message.Body,
                    icon = MaaSConfig.Current.Layout.Images.Favicon,
                    click_action = message.Link,
                },
                to = subscription.Token,
                data = new
                {
                    custom_link = message.Link,
                    custom_icon = MaaSConfig.Current.Layout.Images.Favicon,
                },
                webpush = new
                {
                    fcm_options = new
                    {
                        link = message.Link,
                    }
                }
            };
            request.AddJsonBody(obj);

            return request;
        }
    }
}