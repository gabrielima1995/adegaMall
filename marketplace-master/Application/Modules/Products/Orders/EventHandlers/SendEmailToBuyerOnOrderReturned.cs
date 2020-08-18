using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using System;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderReturned : IDomainEventHandler<OrderStatusChangedEvent>
    {
        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.Returned)
                return;

            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            var result = MessagesContext.Current
                .GenerateAndSend<OrderReturnedMessageToBuyerSpecification, OrderReturnedMessageModel>(
                    new OrderReturnedMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        SellerEmail = order.Seller.Email
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}