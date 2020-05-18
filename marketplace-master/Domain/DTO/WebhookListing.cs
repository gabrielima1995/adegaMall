using Domain.Entities;
using System;

namespace Domain.DTO
{
    public class WebhookListing
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public WebhookEvent EventType { get; set; }

        public string Url { get; set; }

        public WebhookListing()
        {
        }

        public WebhookListing(Webhook webhook)
        {
            Id = webhook.Id;
            CreatedAt = webhook.CreatedAt;
            EventType = webhook.EventType;
            Url = webhook.Url;
        }
    }
}