using System.ComponentModel;

namespace PaymentIntegrator.Core.PaymentMethods
{
    public enum PaymentMethod
    {
        [Description("CREDIT_CARD_LABEL")]
        CreditCard = 0,

        [Description("BOLETO_LABEL")]
        Boleto = 1,

        [Description("OFFLINE_PAYMENT_LABEL")]
        Offline = 2
    }
}