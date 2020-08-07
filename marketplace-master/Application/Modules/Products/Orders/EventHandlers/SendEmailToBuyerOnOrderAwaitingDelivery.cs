using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Framework;
using System;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderAwaitingDelivery : IDomainEventHandler<OrderStatusChangedEvent>
    {
        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus == OrderStatus.AwaitingDelivery)
                SendMessageForDelivery(domainEvent);

            else if (domainEvent.NewStatus == OrderStatus.AwaitingPickup)
                SendMessageForPickup(domainEvent);
        }

        private void SendMessageForDelivery(OrderStatusChangedEvent domainEvent)
        {
            var model = GetMessageModel(domainEvent);
            var result = MessagesContext.Current
                .GenerateAndSend<OrderAwaitingDeliveryMessageToBuyerSpecification, OrderAwaitingDeliveryOrPickupMessageModel>(
                    model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { domainEvent.Order.Buyer.Email }, domainEvent.Order.Buyer.Lang);

            ThrowIfFailure(result, domainEvent);
        }

        private void SendMessageForPickup(OrderStatusChangedEvent domainEvent)
        {
            var model = GetMessageModel(domainEvent);
            var result = MessagesContext.Current
                .GenerateAndSend<OrderAwaitingPickupMessageToBuyerSpecification, OrderAwaitingDeliveryOrPickupMessageModel>(
                    model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { domainEvent.Order.Buyer.Email }, domainEvent.Order.Buyer.Lang);

            ThrowIfFailure(result, domainEvent);
        }

        private OrderAwaitingDeliveryOrPickupMessageModel GetMessageModel(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            return new OrderAwaitingDeliveryOrPickupMessageModel
            {
                BuyerName = order.Buyer.Name,
                OrderId = order.Id.DisplayShortGuid(),
                ShippingAddress = order.ShippingAddress.ToString(),
                SellerAddress = order.Seller.Address?.ToString(),
                SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                SellerEmail = order.Seller.Email
            };
        }

        private void ThrowIfFailure(OperationResult result, OrderStatusChangedEvent domainEvent)
        {
            if (result.IsFailure)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}