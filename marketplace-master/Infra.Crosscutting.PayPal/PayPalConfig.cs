using System;

namespace Infra.Crosscutting.PayPal
{
    public class PayPalConfig : IPayPalConfig
    {
        public string MerchantId { get; }

        public bool IsSandbox { get; }

        public string NotifyUrl { get; }

        public PayPalConfig(string merchantId, bool isSandbox, string notifyUrl)
        {
            MerchantId = merchantId;
            IsSandbox = isSandbox;
            NotifyUrl = notifyUrl;
        }

        public static PayPalConfig Get()
        {
            return new PayPalConfig(
                ConfigHelper.GetSetting("PAYPAL:MERCHANT_ID"),
                ConfigHelper.GetSetting("PAYPAL:SANDBOX:ACTIVE", false),
                ConfigHelper.GetSetting("PAYPAL:NOTIFY_URL"));
        }
    }
}