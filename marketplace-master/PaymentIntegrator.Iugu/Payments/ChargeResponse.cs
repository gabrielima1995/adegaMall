using Newtonsoft.Json;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.ObjectModel;

namespace PaymentIntegrator.Iugu.Payments
{
    public class ChargeResponse : ITransaction
    {
        [JsonProperty("invoice_id")]
        public string TransactionId { get; set; }

        public string Status { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public decimal Amount { get; set; }

        public decimal RefundedAmount { get; private set; }

        public decimal StartupFee { get; set; }

        public decimal RefundedStartupFee { get; private set; }

        public string StartupSplitId { get; set; }

        public decimal GatewayFee { get; set; }

        public decimal RefundedGatewayFee { get; private set; }

        public string PostbackUrl { get; set; }

        [JsonProperty("url")]
        public string BoletoUrl { get; set; }

        public string BoletoBarCode { get; set; }

        public DateTime? BoletoExpirationDate { get; set; }

        public bool IsPaid() => Message?.IndexOf("Autorizado", StringComparison.InvariantCultureIgnoreCase) >= 0;

        public bool IsPendingPayment() => false;

        public bool IsPaymentCancelledOrRejected() => false;

        public int Installments { get; set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; }

        public bool IsMultiPayment => false;

        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            // Usado somente no Pagar.me
            RefundedAmount = refundedAmount;
            RefundedStartupFee = refundedStartupFee;
            RefundedGatewayFee = refundedGatewayFee;
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            throw new InvalidOperationException("ChargeResponse class 'SetValuesForMultiPayment' does not have multipayments.");
        }
    }
}