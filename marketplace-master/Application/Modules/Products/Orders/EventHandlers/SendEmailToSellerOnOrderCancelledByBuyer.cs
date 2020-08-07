using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using Infra.Crosscutting.Globalization;
using System;
using System.Globalization;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToSellerOnOrderCancelledByBuyer : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IPushNotificationService _notificationService;

        public SendEmailToSellerOnOrderCancelledByBuyer(IPushNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.Canceled)
                return;

            if (domainEvent.ActingPerson?.Id == null)
                return;

            if (domainEvent.ActingPerson?.Id != domainEvent.Order.Buyer?.Id)
                return;

            var config = MaaSConfig.Current;
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            _notificationService.NotifyUser(order.Seller.UserId, new PushNotification()
            {
                Title = GlobalizationHelper.GetString("NOTIFICATION_ORDER_CANCELLED_TITLE"),
                Body = string.Format(GlobalizationHelper.GetString("NOTIFICATION_ORDER_CANCELLED_BY_BUYER_DESC"), order.Id.Substring(8)),
                Link = $"/mysales/details/{order.Id}",
            });

            var result = MessagesContext.Current
                .GenerateAndSend<OrderCanceledMessageToSellerSpecification, OrderCanceledMessageModel>(
                    new OrderCanceledMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        Amount = config.DefaultCurrency + " " + order.Amount.ToStringLocalized(lang),
                        ExtraInfo = order.ExtraInfo,
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        CancelExplanation = domainEvent.AdditionalInfo,
                        SellerEmail = order.Seller.Email,
                        BuyerEmail = order.Buyer.Email
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { order.Seller.Email },
                    lang);

            if (!result.OK)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }
    }
}