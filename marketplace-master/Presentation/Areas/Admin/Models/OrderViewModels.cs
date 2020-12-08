using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;

namespace Presentation.Areas.Admin.Models
{
    public class OrderListItemVM
    {
        public OrderListItemVM(Order order, MarketplaceTimeZone timeZone)
        {
            Id = order.Id;
            Buyer = order.Buyer;
            Seller = order.Seller;
            Amount = order.Amount;
            CreatedAt = timeZone.ConvertTimeFromUtc(order.CreatedAt);
            PaymentMethod = order.PaymentMethod;
            Status = order.Status;
            Location = order.ShippingAddress?.CityAndState;
            ShippingAmount = order.Shipping;
            ShippingMethod = order.ShippingMethod;
            StartupFee = order.StartupFee;
            ProductsCount = order.Products?.Count ?? 0;
        }

        public Guid Id { get; set; }
        public string ShortId => Id.ToString().Substring(0, 8);
        public Buyer Buyer { get; set; }
        public string BuyerName => Buyer.FullName;
        public Seller Seller { get; set; }
        public string SellerName => Seller.PublicProfileName;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        public string Location { get; set; }
        public decimal ShippingAmount { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public decimal StartupFee { get; set; }
        public int ProductsCount { get; set; }
        public bool ShowButtonForCheckingTranStatus { get; set; }
    }

    public class OrderDetailsViewModel
    {
        private readonly MarketplaceTimeZone timeZone;

        public OrderDetailsViewModel(OrderDetails orderDetails, MarketplaceTimeZone timeZone)
        {
            var config = MaaSConfig.Current;

            OrderId = orderDetails.Order.Id;
            OrderDate = timeZone.ConvertTimeFromUtc(orderDetails.Order.CreatedAt);
            Title = "Pedido #" + orderDetails.Order.Id.ToString().Substring(0, 8);
            ExtraInfo = orderDetails.Order.ExtraInfo;

            Buyer = orderDetails.Order.Buyer;
            Seller = orderDetails.Order.Seller;
            HistoryEntries = orderDetails.Order.Histories;
            Products = orderDetails.Order.Products;

            ShowShippingDetails = orderDetails.Order.ShippingMethod.IsPhysicalDelivery();
            ShippingMethodDescription = orderDetails.Order.ShippingMethod.DisplayDescription();
            ShippingOut = orderDetails.Order.ShippingMethod.IsShippingOut();

            if (ShowShippingDetails)
            {
                ShippingAddress = orderDetails.Order.ShippingAddress.ToString();

                if (ShippingOut)
                    SellerAddress = orderDetails.Order.Seller.Address.ToString();
            }

            StatusDescription = orderDetails.Order.Status.DisplayDescription();

            HasTransaction = orderDetails.Order.Transaction != null;
            ShowButtonForCheckingTranStatus = (orderDetails.Order.PaymentMethod == PaymentMethod.Boleto || (orderDetails.Order.PaymentMethod == PaymentMethod.CreditCard && config.Payment.GatewayIsWirecard())) && orderDetails.Order.Status == OrderStatus.PendingPayment;
            PaymentMethodDescription = orderDetails.Order.PaymentMethod.DisplayDescription();

            if (HasTransaction)
            {
                InstallmentsCount = orderDetails.Order.Transaction.Installments;

                switch (config.Payment.ActiveGateway)
                {
                    case "moip":
                        GatewayTransactionUrl = "https://conta.wirecard.com.br/orders/list";
                        break;
                    case "pagarme":
                        GatewayTransactionUrl = "https://dashboard.pagar.me/#/transactions/" + orderDetails.Order.Transaction.TransactionId;
                        break;
                    default:
                        throw new Exception("Unrecognized payment gateway: " + config.Payment.ActiveGateway);
                }
            }

            ProductsAmount = orderDetails.Order.SubTotal;
            ShippingAmount = orderDetails.Order.Shipping;
            DiscountAmount = orderDetails.Order.Discount;
            TotalAmount = orderDetails.Order.Amount;
            this.timeZone = timeZone;
        }

        public Guid OrderId { get; }
        public DateTime OrderDate { get; }
        public string Title { get; }
        public string ExtraInfo { get; set; }

        public Buyer Buyer { get; }
        public Seller Seller { get; }
        public IList<OrderHistory> HistoryEntries { get; }
        public IList<OrderProduct> Products { get; }

        public bool ShowShippingDetails { get; }
        public bool ShippingOut { get; }
        public string ShippingMethodDescription { get; }
        public string SellerAddress { get; }
        public string ShippingAddress { get; }

        public string StatusDescription { get; }

        public bool HasTransaction { get; }
        public int InstallmentsCount { get; }
        public string GatewayTransactionUrl { get; }
        public bool ShowButtonForCheckingTranStatus { get; }
        public string PaymentMethodDescription { get; }

        public decimal ProductsAmount { get; }
        public decimal ShippingAmount { get; }
        public decimal DiscountAmount { get; }
        public decimal TotalAmount { get; }

        public DateTime LocalizeDateTime(DateTime utcDateTime) => timeZone.ConvertTimeFromUtc(utcDateTime);
    }
}