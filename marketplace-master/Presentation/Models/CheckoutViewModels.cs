using Application.Config;
using Domain.DTO;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Models
{
    public class CheckoutVM : Checkout
    {
        private MaaSConfig Config => MaaSConfig.Current;

        public string SessionId { get; private set; }

        public bool IsShippingOut => Cart != null && (
            Cart.ShippingMethod == ShippingMethod.AsapLog ||
            Cart.ShippingMethod == ShippingMethod.PostalExpress ||
            Cart.ShippingMethod == ShippingMethod.PostalRegular ||
            Cart.ShippingMethod == ShippingMethod.PrivateFleet);

        public bool ShowBoletoPaymentOption => Config.Payment.Enabled && Config.Payment.PaymentGateway.Boleto.Enabled && (Cart != null && !Cart.Items.All(x => x.Price == 0M));

        public bool ShowCreditCardPaymentOption => Config.Payment.Enabled && Config.Payment.PaymentGateway.CreditCard.Enabled && (Cart != null && !Cart.Items.All(x => x.Price == 0M));

        public string ShippingWarningMessage { get; private set; }

        public bool ShowPaymentMethodOptions
        {
            get
            {
                var count = 0;
                if (ShowBoletoPaymentOption) count++;
                if (ShowCreditCardPaymentOption) count++;
                if (ShowOfflinePaymentOption) count++;
                return count > 1;
            }
        }

        public bool ShowOfflinePaymentOption => Config.Product.Order.OfflinePaymentEnabled;

        public bool ShowShipping
        {
            get
            {
                if (Cart == null)
                    return true;

                var noShippingCosts = Cart.Items.Sum(item => item.Shipping).Equals(0m);
                var isQuoteBilling = MaaSConfig.Current.Quote.Enabled && MaaSConfig.Current.Quote.QuoteBilling.Enabled && Cart.Items.All(item => item.Product.IsBillingProduct.GetValueOrDefault());
                var isActualNoShipping = Cart.ShippingMethod == ShippingMethod.ActualNoShipping;
                var hideShipping = (isQuoteBilling || isActualNoShipping) && noShippingCosts;
                return !hideShipping;
            }
        }

        public CheckoutVM()
        {
        }

        public CheckoutVM(string sessionId)
        {
            SessionId = sessionId;
        }

        public bool ShowInstallmentOptions()
        {
            return Payment.InstallmentOptions.Count > 1;
        }

        public int GetMaxInstallamentsAvailable()
        {
            return Payment.InstallmentOptions.Count;
        }

        public SelectList GetInstallmentsSelectList()
        {
            var p = Payment;

            if (p.InstallmentOptions == null)
                p.InstallmentOptions = new List<InstallmentOption>();

            var textFormat = CultureInfo.CurrentCulture.GetStringLocalized("INSTALLMENT_OPTION_AMOUNT");
            var currency = MaaSConfig.Current.DefaultCurrency;

            var items = p.InstallmentOptions.Select(opt =>
            {
                var itemText = string.Format(textFormat, opt.InstallmentsCount, currency, opt.Amount);
                return new KeyValuePair<int, string>(opt.InstallmentsCount, itemText);
            });

            var selectList = new SelectList(items, "Key", "Value", p.Installments);

            return selectList;
        }

        public static CheckoutVM From(string shippingWarningMessage, Checkout input, string sessionId)
        {
            return new CheckoutVM(sessionId)
            {
                Cart = input.Cart,
                Payment = input.Payment,
                CartValidation = input.CartValidation,
                ShippingWarningMessage = shippingWarningMessage
            };
        }
    }
}