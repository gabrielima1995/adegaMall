using Domain.Entities;

namespace Domain.DTO
{
    public class WebhookCreating
    {
        public WebhookEvent EventType { get; set; }

        public string Url { get; set; }
    }
}