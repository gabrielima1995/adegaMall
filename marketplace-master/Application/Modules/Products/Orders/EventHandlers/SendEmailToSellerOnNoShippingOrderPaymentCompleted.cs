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
    public class SendEmailToSellerOnNoShippingOrderPaymentCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService _notificationService;

        public SendEmailToSellerOnNoShippingOrderPaymentCompleted(IPushNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.CompletedPayment)
                return;

            if (domainEvent.Order.ShippingMethod.NotIn(ShippingMethod.NoShipping, ShippingMethod.ActualNoShipping))
                return;

            SendPush(domainEvent);
            SendEmail(domainEvent);
        }

        private void SendEmail(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            var result = MessagesContext.Current
                .GenerateAndSend<OrderCompletedPaymentMessageToSellerSpecification, OrderCompletedPaymentMessageModel>(
                    new OrderCompletedPaymentMessageModel
                    {
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        BuyerName = order.Buyer.Name,
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        Links = order.Products.Where(p => p.NoShippingUrl != null).Select(p => p.NoShippingUrl),
                        ExtraInfo = order.ExtraInfo
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Seller.Email },
                    lang);

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private void SendPush(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            _notificationService.NotifyUser(domainEvent.Order.Seller.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_ORDER_PAID_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_ORDER_PAID_DESC", MaaSConfig.Current.Localization.DefaultLang), order.Id.Substring(8)),
                Link = $"/mysales/details/{order.OrderId}"
            });
        }
    }
}