using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Framework;
using Infrastructure.Shipping.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2.Infrastructure.Shipping.Adapters
{
    /// <summary>
    /// Adapter para métodos de entrega fora do local de venda.
    /// </summary>
    public abstract class ShippingOutAdapterBase : ShippingMethodAdapterBase, IShippingMethodAdapter
    {
        private readonly MaaSConfig config = MaaSConfig.Current;
        private readonly ShippingMethod shippingMethod;

        protected ShippingOutAdapterBase(ShippingMethod shippingMethod)
        {
            this.shippingMethod = shippingMethod;
        }

        public bool AcceptsTrackingCode => true;

        public void ChangeStatusForNewOrder(Order order, bool paymentUnderReview)
        {
            if (!paymentUnderReview)
            {
                ChangeStatus(order, OrderStatus.PendingPayment);

                if (order.IsPaid)
                    ChangeStatusToOrderPaid(order, true);
            }
        }

        public void ChangeStatusToOrderPaid(Order order, bool deliveryAllowed = true)
        {
            ChangeStatus(order, OrderStatus.CompletedPayment);

            if (deliveryAllowed)
                ChangeStatus(order, OrderStatus.AwaitingDelivery);
        }

        public ShippingMethod GetMethod() => shippingMethod;

        public bool IsOrderCompleted(Order order)
        {
            return (order.Status == OrderStatus.Delivered && order.IsSale) ||
                order.Status == OrderStatus.Canceled ||
                order.Status == OrderStatus.Rejected ||
                order.Status == OrderStatus.Returned /* locação devolvida */;
        }

        public override List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus)
        {
            var list = new List<OrderStatus>();

            if (currentStatus == OrderStatus.Processed)
                list.Add(OrderStatus.PendingPayment);

            if (currentStatus == OrderStatus.PendingPayment)
            {
                list.Add(OrderStatus.CompletedPayment);
                list.Add(OrderStatus.Canceled);
                list.Add(OrderStatus.Rejected);
            }

            if (currentStatus == OrderStatus.CompletedPayment)
                list.Add(OrderStatus.AwaitingDelivery);

            if (currentStatus == OrderStatus.AwaitingDelivery)
                list.Add(OrderStatus.DeliveryOnTheWay);

            if (currentStatus.In(
                OrderStatus.DeliveryOnTheWay,
                OrderStatus.AwaitingDelivery))
                list.Add(OrderStatus.Delivered);

            if (currentStatus.NotIn(
                OrderStatus.Delivered,
                OrderStatus.Canceled,
                OrderStatus.Rejected,
                OrderStatus.Returned))
                list.Add(OrderStatus.Canceled);

            if (currentStatus == OrderStatus.Delivered)
                list.Add(OrderStatus.Rejected);

            if (config.Product.BusinessModel.Rental.Enabled)
                if (currentStatus == OrderStatus.Delivered)
                    list.Add(OrderStatus.Returned);

            return list.Distinct().ToList();
        }

        public abstract OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping);
        public abstract OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, decimal discount, Store store, Address shippingAddress);
        public abstract OperationResult<Uri> GetTrackingLink(string trackingCode);
        public abstract DateTime? CalculateDeliveryDate(int deliveryDays);
    }
}