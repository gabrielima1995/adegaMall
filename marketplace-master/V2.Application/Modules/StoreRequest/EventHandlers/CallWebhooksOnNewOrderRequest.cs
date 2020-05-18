using Domain.Entities;
using Domain.Events;
using V2.Application.Webhooks;

namespace V2.Application.Modules.StoreRequest.EventHandlers
{
    public class CallWebhooksOnNewOrderRequest : IDomainEventHandler<NewOrderRegisteredEvent>
    {
        private readonly IWebhookCallService _webhookCallService;

        public CallWebhooksOnNewOrderRequest(IWebhookCallService webhookCallService)
        {
            _webhookCallService = webhookCallService;
        }

        public void Handle(NewOrderRegisteredEvent domainEvent)
        {
            _webhookCallService.CallByEvent(WebhookEvent.OrderCreated,
                domainEvent.SellerId,
                domainEvent.OrderId.ToString());

            _webhookCallService.CallByEvent(WebhookEvent.All,
                domainEvent.SellerId,
                domainEvent.OrderId.ToString());
        }
    }
}