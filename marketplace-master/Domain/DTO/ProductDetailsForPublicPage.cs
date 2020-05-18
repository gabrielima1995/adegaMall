using Application.Config;
using Domain.Entities.StoreModule;
using Infra.Crosscutting.Globalization;
using System;

namespace Domain.DTO
{
    public class ProductDetailsForPublicPage
    {
        public Product Product { get; set; }
        public ProductPaymentOptions PaymentOptions { get; set; }
    }

    public class ProductPaymentOptions
    {
        public bool CreditCardEnabled { get; set; }
        public bool BoletoEnabled { get; set; }

        public int MaxInstallments { get; set; }
        public decimal InstallmentAmount { get; set; }

        public string GetDescription()
        {
            string msg = null;
            var currencySymbol = MaaSConfig.Current.DefaultCurrency;

            if (CreditCardEnabled && MaxInstallments > 1)
                msg = string.Format(GlobalizationHelper.GetString("PRODUCT_PAYMENT_INFO_INSTALLMENTS"), MaxInstallments, currencySymbol, InstallmentAmount.DisplayDecimal(2));

            if (BoletoEnabled)
            {
                if (msg != null) msg += " ";
                msg += GlobalizationHelper.GetString("PRODUCT_PAYMENT_INFO_BOLETO_ENABLED");
            }

            return msg;
        }
    }
}
