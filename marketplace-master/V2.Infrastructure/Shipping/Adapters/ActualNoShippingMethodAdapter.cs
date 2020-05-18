using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Infrastructure.Shipping.Adapters
{
    public class ActualNoShippingMethodAdapter : ShippingMethodAdapterBase, IShippingMethodAdapter
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        public bool AcceptsTrackingCode => false;

        public DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            throw new InvalidOperationException("Shipping method 'ActualNoShipping' does not have shipping days.");
        }

        public void ChangeStatusForNewOrder(Order order)
        {
            ChangeStatus(order, OrderStatus.PendingPayment);

            if (order.IsPaid)
                ChangeStatusToOrderPaid(order);
        }

        public void ChangeStatusToOrderPaid(Order order, bool deliveryAllowed = true)
        {
            ChangeStatus(order, OrderStatus.CompletedPayment);
        }

        public ShippingMethod GetMethod()
        {
            return ShippingMethod.ActualNoShipping;
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            if (config.Product.ShippingMode == ShippingMode.PerProduct)
            {
                if (product.Product.ActualNoShippingEnabled)
                    return CorreiosShippingCollection.Empty();

                return new ErrorDictionary(
                    product.Product.Id.ToString(), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_ACTUAL_NO_SHIPPING_NOT_ENABLED"));
            }

            return CorreiosShippingCollection.Empty();
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, Store store, Address shippingAddress)
        {
            if (config.Product.ShippingMode == ShippingMode.PerStore)
                return new ErrorDictionary(
                    store.SellerId, CultureInfo.CurrentCulture.GetStringLocalized("STORE_ACTUAL_NO_SHIPPING_NOT_SUPPORTED"));

            return CorreiosShippingCollection.Empty();
        }

        public OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            return new OperationResult<Uri>("", GlobalizationHelper.GetString("DELIVERY_METHOD_DOES_NOT_SHIP"));
        }

        public bool IsOrderCompleted(Order order)
        {
            return order.Status.In(OrderStatus.Completed, OrderStatus.Canceled);
        }

        public override List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus)
        {
            var list = new List<OrderStatus>();

            if (currentStatus == OrderStatus.Processed)
                list.Add(OrderStatus.PendingPayment);

            if (currentStatus == OrderStatus.PendingPayment)
                list.Add(OrderStatus.CompletedPayment);

            if (currentStatus != OrderStatus.Canceled)
                list.Add(OrderStatus.Canceled);

            return list;
        }
    }
}