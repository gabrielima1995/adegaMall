using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using System.Collections.Generic;

namespace PaymentIntegrator.Moip.Payments
{
    public class MoipPayment
    {
        [JsonProperty(PropertyName = "installmentCount")]
        public int InstallmentCount { get; set; }

        [JsonProperty(PropertyName = "fundingInstrument", NullValueHandling = NullValueHandling.Ignore)]
        public object FundingInstrument { get; set; }

        [JsonProperty(PropertyName = "escrow", NullValueHandling = NullValueHandling.Ignore)]
        public object Escrow { get; set; }

        [JsonProperty(PropertyName = "statementDescriptor", NullValueHandling = NullValueHandling.Ignore)]
        public string StatementDescriptor { get; set; }


        public static MoipPayment CreateMultiPayment(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions, IAddress address)
        {
            var payment = new MoipPayment();
            payment.InstallmentCount = paymentOptions.Installments;
            payment.FundingInstrument = MoipPaymentMethods.CreatePaymentMethod(paymentOptions, customer);
            payment.StatementDescriptor = MoipConfig.Current.StatementDescription;

            if (MoipConfig.Current.PaymentWarrantyEnabled)
            {
                payment.Escrow = new
                {
                    description = "Pagamento em garantia",
                };
            }

            return payment;
        }
    }
}
