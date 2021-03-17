using Domain.Entities.StoreModule;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Views.Products.Cart
{
    public class CheckoutConfirmationViewModel
    {
        public CheckoutConfirmationViewModel(IEnumerable<Order> orders)
        {
            if (orders == null || orders.Count() == 0)
                throw new ArgumentException("Cannot create a ViewModel for Checkout Confirmation from an empty list of orders.", nameof(orders));

            Orders = orders.ToList();
            ReferenceOrder = Orders.First();
        }

        public IReadOnlyList<Order> Orders { get; }
        public Order ReferenceOrder { get; }

        public bool HasMultipleSellers => Orders.Count > 1;
        public bool IsBoleto => ReferenceOrder.PaymentMethod == PaymentMethod.Boleto;
        public bool IsCreditCard => ReferenceOrder.PaymentMethod == PaymentMethod.CreditCard;
        public bool IsOfflinePayment => ReferenceOrder.PaymentMethod == PaymentMethod.Offline;
        public bool IsPendingPayment => !ReferenceOrder.IsPaid;
    }
}