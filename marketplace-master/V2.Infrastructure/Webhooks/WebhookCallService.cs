using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using Hangfire;
using Infrastructure.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using V2.Application.Webhooks;

namespace V2.Infrastructure.Webhooks
{
    public class WebhookCallService : IWebhookCallService
    {
        private readonly IWebhookRepository _webhookRepository;
        private readonly IBackgroundWorker _backgroundWorker;

        public WebhookCallService(
            IWebhookRepository webhookRepository,
            IBackgroundWorker backgroundWorker)
        {
            _webhookRepository = webhookRepository;
            _backgroundWorker = backgroundWorker;
        }

        private string BuildBody(WebhookToNotify webhook, string resourceId)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new StringSnakeCaseEnumConverter());

            return JsonConvert.SerializeObject(new
            {
                webhook.EventType,
                ResourceId = resourceId
            }, settings);
        }

        private void SetSecurityHeaders(HttpWebRequest request, WebhookToNotify webhook)
        {
            var code = Guid.NewGuid().ToString("N").Substring(0, 8);
            var hash = MD5.Encode(webhook.ApiToken + code);

            request.Headers.Add("X-VALIDATION-CODE", code);
            request.Headers.Add("X-VALIDATION-HASH", hash);
        }

        private HttpWebRequest CreateRequest(WebhookToNotify webhook, string resourceId)
        {
            var request = WebRequest.CreateHttp(webhook.Url);
            request.UserAgent = "SaaS Marketplace INA v2";
            request.Method = "POST";
            request.Timeout = 20000;
            request.ContentType = "application/json";

            var data = BuildBody(webhook, resourceId);

            SetSecurityHeaders(request, webhook);

            using (var s = request.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(data);

            return request;
        }

        private bool IsResponseOk(HttpWebResponse response)
        {
            return response.StatusCode.In(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        private HttpWebResponse ProcessRequest(HttpWebRequest request)
        {
            return (HttpWebResponse)request.GetResponse();
        }

        public void CallByEvent(WebhookEvent eventType, string sellerId, string resourceId)
        {
            var webhooks = _webhookRepository.ListToNotify(eventType, sellerId);

            foreach (var webhook in webhooks)
            {
                _backgroundWorker.Schedule<WebhookCallService>(
                    i => i.Call(webhook, resourceId),
                    TimeSpan.FromMinutes(ConfigHelper.GetSetting("WEBHOOKS:NOTIFY:DELAY", 1)));
            }
        }

        private void SaveLog(WebhookToNotify webhook, string resourceId, WebhookLogRequest webhookLogRequest, WebhookLogResponse webhookLogResponse)
        {
            var retryCount = 0; // TODO: resolver, antes era assim [Hangfire.Servier.PlatformContext], tinha dependência, mas context era sempre passado como null -> context.GetJobParameter<int>("RetryCount");

            // Creates webhook log entry.
            var webhookLogEntry = new WebhookLogEntry(Environment.MachineName, webhook.SellerId, resourceId,
                webhook.EventType, retryCount + 1,
                webhookLogRequest, webhookLogResponse);

            // Enqueues save operation.
            JobHelper.Enqueue<WebhookLogService>(x => x.Save(webhookLogEntry));
        }

        public void Call(WebhookToNotify webhook, string resourceId)
        {
            var request = CreateRequest(webhook, resourceId);

            // Gets webhook call data.
            var webhookLogRequest = WebhookLogRequest.New(request, webhook.ApiToken, BuildBody(webhook, resourceId));

            try
            {
                // Process request.
                using (var response = ProcessRequest(request))
                {
                    // Gets response data.
                    var webhookLogResponse = WebhookLogResponse.New(response);

                    // Saves log.
                    SaveLog(webhook, resourceId, webhookLogRequest, webhookLogResponse);

                    // Checks response.
                    if (!IsResponseOk(response))
                    {
                        // Throws to retry.
                        throw new Exception(
                            $"Response status code: ({(int)response.StatusCode}) {response.StatusCode.ToString()}");
                    }
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                // Gets response data.
                var webhookLogResponse = WebhookLogResponse.New(response);

                // Saves log.
                SaveLog(webhook, resourceId, webhookLogRequest, webhookLogResponse);

                // Throws to retry.
                throw new Exception(
                    $"Response status code: ({(int)response.StatusCode}) {response.StatusCode.ToString()}");
            }
        }
    }
}