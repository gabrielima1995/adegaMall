using PaymentIntegrator.Moip.Common;
using PaymentIntegrator.Moip.Utils;
using RestSharp;
using System;
using System.Net;

namespace PaymentIntegrator.Moip
{
    public static class MoipInitializer
    {
        public static void Initialize(MoipInitializationModel initializationModel)
        {
            ConfigureTls();
            InitializeAppId(initializationModel);
            InitializeStartupReceiverId();
            InitializeNotificationPreferences();
        }

        private static void ConfigureTls()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Ssl3;
        }

        private static void InitializeAppId(MoipInitializationModel initializationModel)
        {
            if (string.IsNullOrWhiteSpace(MoipConfig.Current.AppId))
            {
                var createAppRequest = new MoipRequest("channels", Method.POST);
                createAppRequest.AddJsonBody(initializationModel.GetJsonForApiRequest());

                var response = MoipClient.SendRequest(createAppRequest);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    dynamic obj = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize(response.Content);
                    MoipConfig.Current.AppId = obj.id;
                    MoipConfig.Current.AppAccessToken = obj.accessToken;
                    MoipConfig.Current.AppSecret = obj.secret;
                }
                else
                {
                    throw new Exception($"Error initializing Wirecard App - HTTP {response.ErrorMessage}");
                }
            }
        }

        private static void InitializeNotificationPreferences()
        {
            if (string.IsNullOrWhiteSpace(MoipConfig.Current.NotificationRule))
            {
                var createWebhookRequest = new MoipRequest($"preferences/{MoipConfig.Current.AppId}/notifications", Method.POST);
                createWebhookRequest.AddJsonBody(new
                {
                    events = new[] { "MULTIORDER.PAID" },
                    target = MoipConfig.Current.PostbackUrl + "moip/transactionpostback",
                    media = "WEBHOOK",
                });

                var createWebhookResult = MoipClient.SendOAuthRequest(createWebhookRequest);
                if (createWebhookResult.StatusCode == HttpStatusCode.Created || createWebhookResult.StatusCode == HttpStatusCode.OK)
                {
                    dynamic webhook = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize(createWebhookResult.Content);
                    MoipConfig.Current.NotificationRule = (string)webhook.id;
                }

                var createV1WebhookRequest = new MoipRequest("users/preferences/", Method.POST);
                createV1WebhookRequest.AddJsonBody(new
                {
                    notification = new { webhook = new { url = MoipConfig.Current.PostbackUrl + "moip/subscriptionpostback" } }
                });

                MoipClient.SendV1Request(createV1WebhookRequest);
            }
        }

        private static void InitializeStartupReceiverId()
        {
            if (string.IsNullOrWhiteSpace(MoipConfig.Current.StartupReceiverId))
            {
                var startupRecipientRequest = new MoipRequest("accounts", Method.GET);
                var response = MoipClient.SendRequest(startupRecipientRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic responseObject = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize(response.Content);
                    var receiverId = (string)responseObject.id;

                    if (!string.IsNullOrEmpty(receiverId))
                        MoipConfig.Current.StartupReceiverId = receiverId;
                }
            }
        }
    }
}