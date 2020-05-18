using Domain.Entities;
using System;

namespace Domain.DTO
{
    public class WebhookToNotify
    {
        public Guid Id { get; set; }

        public WebhookEvent EventType { get; set; }

        public string Url { get; set; }

        public string ApiToken { get; set; }

        public string SellerId { get; set; }
    }
}