using Domain.Entities;
using Domain.Events;
using Domain.Events.StoreModule;
using System;
using V2.Application.Webhooks;

namespace V2.Application.Modules.StoreRequest.EventHandlers
{
    public class CallWebhooksOnRentalCreatedUpdatedOrCancelled :
        IDomainEventHandler<NewOrderRegisteredEvent>,
        IDomainEventHandler<RentalInfoUpdated>,
        IDomainEventHandler<RentalCancelled>
    {
        private readonly IWebhookCallService webhookCallService;

        public CallWebhooksOnRentalCreatedUpdatedOrCancelled(IWebhookCallService webhookCallService)
        {
            this.webhookCallService = webhookCallService;
        }

        public void Handle(NewOrderRegisteredEvent domainEvent)
        {
            if (!domainEvent.IsRental)
                Call(domainEvent.OrderId, domainEvent.SellerId, WebhookEvent.RentalAdded);
        }

        public void Handle(RentalInfoUpdated domainEvent) =>
            Call(domainEvent.OrderId, domainEvent.SellerId, WebhookEvent.RentalUpdated);

        public void Handle(RentalCancelled domainEvent) =>
            Call(domainEvent.Order.Id, domainEvent.Order.Seller.Id, WebhookEvent.RentalCancelled);

        private void Call(Guid orderId, string sellerId, WebhookEvent eventType) =>
            webhookCallService.CallByEvent(eventType, sellerId, orderId.ToString());
    }
}