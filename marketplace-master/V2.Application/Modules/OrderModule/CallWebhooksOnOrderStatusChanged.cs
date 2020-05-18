using Domain.Entities;
using Domain.Events;
using V2.Application.Webhooks;

namespace V2.Application.Modules.OrderModule
{
    public class CallWebhooksOnOrderStatusChanged : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IWebhookCallService _webhookCallService;

        public CallWebhooksOnOrderStatusChanged(IWebhookCallService webhookCallService)
        {
            _webhookCallService = webhookCallService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            _webhookCallService.CallByEvent(WebhookEvent.OrderStatusChanged,
                domainEvent.Order.Seller.Id,
                domainEvent.Order.Id.ToString());

            _webhookCallService.CallByEvent(WebhookEvent.All,
                domainEvent.Order.Seller.Id,
                domainEvent.Order.Id.ToString());
        }
    }
}
