using Domain.Entities;
using System;

namespace Domain.DTO
{
    public class WebhookEditing
    {
        public Guid Id { get; set; }

        public WebhookEvent EventType { get; set; }

        public string Url { get; set; }
    }
}