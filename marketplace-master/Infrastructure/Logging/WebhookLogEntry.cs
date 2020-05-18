using Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.Logging
{
    /// <summary>
    /// The webhook log entry.
    /// </summary>
    public class WebhookLogEntry
    {
        /// <summary>
        /// The ID for the webhook log entry.
        /// </summary>
        public Guid WebhookLogEntryId { get; set; }

        /// <summary>
        /// The machine that made the request.
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// The seller owner of the webhook.
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// The resource id of webhook request.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// The event type of webhook.
        /// </summary>
        public WebhookEvent EventType { get; set; }

        /// <summary>
        /// The number of attempts on request.
        /// </summary>
        public int Attempt { get; set; }

        /// <summary>
        /// The webhook log request.
        /// </summary>
        public WebhookLogRequest Request { get; set; }

        /// <summary>
        /// The webhook log response.
        /// </summary>
        public WebhookLogResponse Response { get; set; }

        public WebhookLogEntry()
        {
            WebhookLogEntryId = Guid.NewGuid();
        }

        public WebhookLogEntry(string machine, string seller, string resourceId, WebhookEvent eventType, int attempt,
            WebhookLogRequest request, WebhookLogResponse response)
            : this()
        {
            Machine = machine;
            Seller = seller;
            ResourceId = resourceId;
            EventType = eventType;
            Attempt = attempt;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Creates file name to blob.
        /// </summary>
        /// <returns></returns>
        public string CreateFilename()
        {
            return
                "webhook-call-" +
                Request.Timestamp.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture) +
                "-" +
                WebhookLogEntryId +
                ".json";
        }

        /// <summary>
        /// Creates stream to blob content.
        /// </summary>
        /// <returns></returns>
        public Stream ConvertToStream()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new StringSnakeCaseEnumConverter());

            var data = JsonConvert.SerializeObject(
                this,
                settings);

            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }
    }
}