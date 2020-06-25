using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public class CreditsPackTransaction : BaseEntity<Guid>, ITransaction
    {
        public string TransactionId { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public decimal Amount { get; set; }

        public decimal RefundedAmount { get; set; }

        public decimal StartupFee { get; set; }

        public decimal RefundedStartupFee { get; set; }

        public string StartupSplitId { get; set; }

        public decimal GatewayFee { get; set; }

        public decimal RefundedGatewayFee { get; set; }

        public string PostbackUrl { get; set; }

        public string BoletoUrl { get; set; }

        public string BoletoBarCode { get; set; }

        public DateTime? BoletoExpirationDate { get; set; }

        public int Installments { get; set; }

        public CreditsBundle CreditsBundle { get; set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; }

        public bool IsMultiPayment { get; }

        public bool IsPaid() => Status.Equals("paid", StringComparison.OrdinalIgnoreCase);

        public bool IsPendingPayment() => false;

        public bool IsPaymentCancelledOrRejected() => false;

        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            RefundedAmount = refundedAmount;
            RefundedStartupFee = refundedStartupFee;
            RefundedGatewayFee = refundedGatewayFee;
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            TransactionId = id;
            Amount = amount;
            StartupFee = startupFee;
        }

        public CreditsPackTransaction()
        {
            Id = Guid.NewGuid();
        }
    }
}
