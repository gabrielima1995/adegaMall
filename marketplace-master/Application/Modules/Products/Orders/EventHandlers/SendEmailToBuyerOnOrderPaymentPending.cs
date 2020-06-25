using Application.Config;
using Domain.Core;
using Domain.Entities.StoreModule;
using Domain.Events;
using System;
using System.Globalization;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToBuyerOnOrderPaymentPending : IDomainEventHandler<OrderStatusChangedEvent>
    {
        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.Order.Status != OrderStatus.PendingPayment)
                return;

            var config = MaaSConfig.Current;
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;

            var result = MessagesContext.Current
                .GenerateAndSend<OrderProcessingPaymentMessageToBuyerSpecification, OrderProcessingPaymentMessageModel>(
                    new OrderProcessingPaymentMessageModel
                    {
                        BuyerName = order.Buyer.Name,
                        OrderId = order.Id.DisplayShortGuid(),
                        OrderStatus = order.Status.GetDescriptionLocalized(lang),
                        SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                        Links = order.Products.Where(i => !string.IsNullOrWhiteSpace(i.NoShippingUrl)).Select(i => i.NoShippingUrl),
                        Amount = config.DefaultCurrency + " " + order.Amount.ToStringLocalized(lang),
                        Products = order.Products.Select(i => new OrderProductMessageModel
                        {
                            Item = i.FullItemName,
                            Quantity = i.Quantity.ToStringLocalized(lang, "N0"),
                            Amount = config.DefaultCurrency + " " + i.Amount.ToStringLocalized(lang)
                        }),
                        Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + lang,
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