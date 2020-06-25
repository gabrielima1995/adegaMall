using Application.Config;
using Dapper;
using Domain.Core;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Events;
using Framework;
using System;
using System.Data.SqlClient;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class SendEmailToSellerOnOrderAwaitingDelivery : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly ConnectionString connectionString;

        public SendEmailToSellerOnOrderAwaitingDelivery(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.Order.Status == OrderStatus.AwaitingDelivery)
                SendMessageForDelivery(domainEvent);

            else if (domainEvent.Order.Status == OrderStatus.AwaitingPickup)
                SendMessageForPickup(domainEvent);
        }

        private void SendMessageForDelivery(OrderStatusChangedEvent domainEvent)
        {
            var model = GetMessageModel(domainEvent);
            var result = MessagesContext.Current
                .GenerateAndSend<OrderAwaitingDeliveryMessageToSellerSpecification, OrderAwaitingDeliveryOrPickupMessageModel>(
                    model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { model.SellerEmail }, domainEvent.Order.Buyer.Lang);

            ThrowIfFailure(domainEvent, result);
        }

        private void SendMessageForPickup(OrderStatusChangedEvent domainEvent)
        {
            var model = GetMessageModel(domainEvent);
            var result = MessagesContext.Current
                .GenerateAndSend<OrderAwaitingPickupMessageToSellerSpecification, OrderAwaitingDeliveryOrPickupMessageModel>(
                    model, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { model.SellerEmail }, domainEvent.Order.Buyer.Lang);

            ThrowIfFailure(domainEvent, result);
        }

        private OrderAwaitingDeliveryOrPickupMessageModel GetMessageModel(OrderStatusChangedEvent domainEvent)
        {
            var order = domainEvent.Order;
            var lang = order.Buyer.Lang;
            var emailData = FetchEmailData(domainEvent.Order.Id);

            return new OrderAwaitingDeliveryOrPickupMessageModel
            {
                OrderId = emailData.OrderId.DisplayShortGuid(),
                BuyerName = emailData.BuyerName,
                SellerName = emailData.SellerName,
                SellerEmail = emailData.SellerEmail,
                ShippingAddress = emailData.ShippingAddress?.ToString(),
                SellerAddress = emailData.SellerAddress?.ToString(),
                Link = MaaSConfig.Current.Environment.AppUrl + "mysales/details/" + order.Id + "/?lang=" + lang
            };
        }

        private EmailData FetchEmailData(Guid orderId)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                return conn.Query<DbDataFirstPart, Address, Address, EmailData>(sql,
                    (firstPart, shippingAddress, sellerAddress) => new EmailData
                    {
                        OrderId = firstPart.OrderId,
                        BuyerName = firstPart.BuyerName,
                        SellerName = firstPart.SellerName,
                        SellerEmail = firstPart.SellerEmail,
                        ShippingAddress = shippingAddress,
                        SellerAddress = sellerAddress
                    }, new { orderId }, splitOn: "Id").Single();
            }
        }

        private void ThrowIfFailure(OrderStatusChangedEvent domainEvent, OperationResult result)
        {
            if (result.IsFailure)
                throw new DomainEventHandlerFailedException(GetType(), domainEvent, "Failed on MessagesContext.Current.GenerateAndSend. Details: " + result.Errors.ToSimpleString());
        }

        private class EmailData
        {
            public Guid OrderId { get; set; }
            public string BuyerName { get; set; }
            public string SellerName { get; set; }
            public string SellerEmail { get; set; }
            public Address ShippingAddress { get; set; }
            public Address SellerAddress { get; set; }
        }

        private class DbDataFirstPart
        {
            public Guid OrderId { get; set; }
            public string BuyerName { get; set; }
            public string SellerName { get; set; }
            public string SellerEmail { get; set; }
        }

        private const string sql = @"
            SELECT
	            TheOrder.Id OrderId,
	            Buyer.FullName BuyerName,
	            ISNULL(Seller.AlternativeName, Seller.FullName) SellerName,
	            Seller.Email SellerEmail,
	            ShippingAddress.*,
	            SellerAddress.*
            FROM [Order] TheOrder
            INNER JOIN Person Buyer ON TheOrder.Buyer_Id = Buyer.Id
            INNER JOIN Person Seller ON TheOrder.Seller_Id = Seller.Id
            LEFT JOIN [Address] ShippingAddress ON TheOrder.ShippingAddress_Id = ShippingAddress.Id
            LEFT JOIN [Address] SellerAddress ON Seller.Address_Id = SellerAddress.Id
            WHERE TheOrder.Id = @orderId";
    }
}