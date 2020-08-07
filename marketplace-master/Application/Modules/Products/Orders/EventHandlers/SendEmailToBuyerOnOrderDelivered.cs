using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Infra.Crosscutting.Globalization;
using System;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderDelivered : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService _notificationService;

        public SendEmailToBuyerOnOrderDelivered(IPushNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.Delivered)
                return;

            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            _notificationService.NotifyUser(order.Buyer.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_ORDER_DELIVERED_TITLE"),
                Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_ORDER_DELIVERED_DESC"), order.Id.Substring(8)),
                Link = $"/myorders/details/{order.Id}",
            });

            var result = MessagesContext.Current
                .GenerateAndSend<OrderDeliveredMessageToBuyerSpecification, OrderDeliveredMessageModel>(
                    new OrderDeliveredMessageModel
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