using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IShippingMethodAdapter
    {
        bool AcceptsTrackingCode { get; }

        void ChangeStatusForNewOrder(Order order, bool paymentUnderReview);

        void ChangeStatusToOrderPaid(Order order, bool deliveryAllowed = true);

        OperationResult ChangeStatus(Order order, OrderStatus newStatus, Person person = null, string additionalInfo = null);

        ShippingMethod GetMethod();

        OperationResult<Uri> GetTrackingLink(string trackingCode);

        OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping);

        OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, decimal discount, Store store, Address shippingAddress);
        
        DateTime? CalculateDeliveryDate(int deliveryDays);

        bool IsOrderCompleted(Order order);

        List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus);
    }

    public class ProductWithQty
    {
        public Product Product { get; private set; }

        public int Quantity { get; private set; }

        public DateTime? RentalStart { get; private set; }

        public ProductWithQty(Product product, int quantity = 1, DateTime? rentalStart = null)
        {
            Product = product;
            Quantity = quantity;
            RentalStart = rentalStart;
        }
    }
}