using Domain.DTO;
using Domain.Entities;

namespace V2.Application.Webhooks
{
    public interface IWebhookCallService
    {
        void Call(WebhookToNotify webhook, string resourceId);

        void CallByEvent(WebhookEvent eventType, string sellerId, string resourceId);
    }
}