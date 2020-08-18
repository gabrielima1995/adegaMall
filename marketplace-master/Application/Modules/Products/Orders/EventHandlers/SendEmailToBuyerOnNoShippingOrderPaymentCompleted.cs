using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnNoShippingOrderPaymentCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService notificationService;

        public SendEmailToBuyerOnNoShippingOrderPaymentCompleted(IPushNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (ShouldIgnore(domainEvent))
                return;

            SendPush(domainEvent);
            SendEmail(domainEvent);
        }

        private bool ShouldIgnore(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.CompletedPayment)
                return true;

            if (domainEvent.Order.ShippingMethod.NotIn(ShippingMethod.NoShipping, ShippingMethod.ActualNoShipping))
                return true;

            if (domainEvent.Order.Products.Any(p => p.Product?.IsBillingProduct ?? false))
                return true;

            return false;
        }

        private void SendEmail(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            var result = MessagesContext.Current
                .GenerateAndSend<OrderCompletedPaymentMessageToBuyerSpecification, OrderCompletedPaymentMessageModel>(
                    new OrderCompletedPaymentMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        Links = order.Products.Select(i => i.NoShippingUrl),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerEmail = order.Seller.Email,
                        ExtraInfo = order.ExtraInfo
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email }, lang);

            if (!result.OK)
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