using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Infra.Crosscutting.Globalization;
using System;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderDeliveryOnTheWay : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService _notificationService;
        private readonly IShippingService _shippingService;

        public SendEmailToBuyerOnOrderDeliveryOnTheWay(IPushNotificationService notificationService, IShippingService shippingService)
        {
            _notificationService = notificationService;
            _shippingService = shippingService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.Order.Status != OrderStatus.DeliveryOnTheWay)
                return;

            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            _notificationService.NotifyUser(order.Buyer.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_ORDER_ONDELIVERY_TITLE", MaaSConfig.Current.Localization.DefaultLang),
                Body = GlobalizationHelper.GetString("NOTIFICATION_ORDER_ONDELIVERY_DESC", MaaSConfig.Current.Localization.DefaultLang),
                Link = $"myorders/details/{order.Id}",
            });

            var trackingLink = _shippingService.GetAdapter(order.ShippingMethod).GetTrackingLink(order.TrackingCode);
            var trackingLinkString = trackingLink.OK ? trackingLink.Result.ToString() : trackingLink.Errors.ToSimpleString();

            var result = MessagesContext.Current
                .GenerateAndSend<OrderDeliveryOnTheWayMessageToBuyerSpecification, OrderDeliveryOnTheWayMessageModel>(
                    new OrderDeliveryOnTheWayMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
                        SellerEmail = order.Seller.Email,
                        TrackingLink = trackingLinkString
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Buyer.Email },
                    lang);

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}