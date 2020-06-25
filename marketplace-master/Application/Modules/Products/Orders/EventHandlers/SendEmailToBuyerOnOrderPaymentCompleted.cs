using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderPaymentCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService notificationService;

        public SendEmailToBuyerOnOrderPaymentCompleted(IPushNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.CompletedPayment)
                return;

            SendPush(domainEvent);
            SendEmail(domainEvent);
        }

        private void SendEmail(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            var links = new List<string>
            {
                MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang
            };

            var result = MessagesContext.Current
                .GenerateAndSend<OrderCompletedPaymentMessageToBuyerSpecification, OrderCompletedPaymentMessageModel>(
                    new OrderCompletedPaymentMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        Links = links,
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerEmail = order.Seller.Email,
                        ExtraInfo = order.ExtraInfo
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (result.IsFailure)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private void SendPush(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            notificationService.NotifyUser(order.Seller.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_ORDER_PAID_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_ORDER_PAID_DESC", MaaSConfig.Current.Localization.DefaultLang), order.Id.Substring(8)),
                Link = $"myorders/details/{order.Id}",
            });
        }
    }
}