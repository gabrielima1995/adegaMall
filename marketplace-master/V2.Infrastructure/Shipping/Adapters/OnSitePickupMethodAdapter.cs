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
using System.Linq;

namespace Infrastructure.Shipping.Adapters
{
    public sealed class OnSitePickUpMethodAdapter : ShippingMethodAdapterBase, IShippingMethodAdapter
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        public bool AcceptsTrackingCode => false;

        public DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            throw new InvalidOperationException("Shipping method 'OnSitePickUpMethodAdapter' does not have shipping days.");
        }

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
                ChangeStatus(order, OrderStatus.AwaitingPickup);
        }

        public ShippingMethod GetMethod()
        {
            return ShippingMethod.OnSitePickup;
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            if (config.Product.ShippingMode == ShippingMode.PerProduct)
            {
                if (product.Product.OnSitePickupEnabled) return CorreiosShippingCollection.Empty();
                return new ErrorDictionary(product.Product.Id.ToString(), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_ON_SITE_PICKUP_NOT_ENABLED"));
            }

            else return CorreiosShippingCollection.Empty();
        }

        public OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, Store store, Address shippingAddress)
        {
            if (config.Product.ShippingMode == ShippingMode.PerStore)
            {
                if (store.OnSitePickupEnabled) return CorreiosShippingCollection.Empty();
                return new ErrorDictionary(store.SellerId, CultureInfo.CurrentCulture.GetStringLocalized("STORE_ON_SITE_PICKUP_NOT_ENABLED"));
            }

            else return CorreiosShippingCollection.Empty();
        }

        public OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            return new OperationResult<Uri>("", GlobalizationHelper.GetString("DELIVERY_METHOD_DOES_NOT_SHIP"));
        }

        public bool IsOrderCompleted(Order order)
        {
            return (order.Status == OrderStatus.Delivered && order.IsSale) ||
                order.Status == OrderStatus.Canceled ||
                order.Status == OrderStatus.Rejected ||
                order.Status == OrderStatus.Returned;
        }

        public override List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus)
        {
            var list = new List<OrderStatus>();

            if (currentStatus == OrderStatus.Processed)
                list.Add(OrderStatus.PendingPayment);

            if (currentStatus == OrderStatus.PendingPayment)
                list.Add(OrderStatus.CompletedPayment);

            if (currentStatus == OrderStatus.CompletedPayment)
                list.Add(OrderStatus.AwaitingPickup);

            if (currentStatus == OrderStatus.AwaitingPickup)
                list.Add(OrderStatus.Delivered);

            if (currentStatus != OrderStatus.Delivered &&
                currentStatus != OrderStatus.Canceled &&
                currentStatus != OrderStatus.Rejected &&
                currentStatus != OrderStatus.Returned)
                list.Add(OrderStatus.Canceled);

            if (currentStatus == OrderStatus.Delivered)
                list.Add(OrderStatus.Rejected);

            if (config.Product.BusinessModel.Rental.Enabled)
                if (currentStatus == OrderStatus.Delivered)
                    list.Add(OrderStatus.Returned);

            return list.Distinct().ToList();
        }
    }
}