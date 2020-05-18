using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class WebhookCalledApiLog : BaseEntity<Guid>
    {
        public string Token { get; private set; }

        public string Seller { get; private set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateTime { get; private set; }

        public string Url { get; private set; }

        public WebhookEvent EventType { get; private set; }

        public string EntityRef { get; private set; }

        public string HttpVerb { get; private set; }

        public string HttpStatusCode { get; private set; }

        public int Attempt { get; private set; }

        private WebhookCalledApiLog() { }

        public static WebhookCalledApiLog New(Guid id, string token, string seller, DateTime dateTime, string url,
            WebhookEvent eventType, string entityRef, string httpVerb, string httpStatusCode, int attempt)
        {
            return new WebhookCalledApiLog
            {
                Id = id,
                Token = token,
                Seller = seller,
                DateTime = dateTime,
                Url = url,
                EventType = eventType,
                EntityRef = entityRef,
                HttpVerb = httpVerb,
                HttpStatusCode = httpStatusCode,
                Attempt = attempt
            };
        }
    }
}