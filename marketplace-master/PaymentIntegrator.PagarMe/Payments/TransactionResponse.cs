using PagarMe;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PaymentIntegrator.PagarMe.Payments
{
    /// <summary>
    /// Representa um transação já salva no Pagar.me.
    /// </summary>
    public class TransactionResponse : ITransaction
    {
        public string TransactionId { get; private set; }

        public string Status { get; private set; }

        public string Message { get; private set; }

        public decimal Amount { get; private set; }

        public decimal RefundedAmount { get; private set; }

        public decimal StartupFee { get; private set; }

        public decimal RefundedStartupFee { get; private set; }

        public string StartupSplitId { get; private set; }

        public decimal GatewayFee { get; private set; }

        public decimal RefundedGatewayFee { get; private set; }

        public string PostbackUrl { get; private set; }

        public string BoletoUrl { get; private set; }

        public string BoletoBarCode { get; private set; }

        public DateTime? BoletoExpirationDate { get; private set; }

        public int Installments { get; private set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; set; }

        public bool IsMultiPayment => false;

        public TransactionResponse(Transaction transaction)
        {
            TransactionId = transaction.Id;
            Status = transaction.Status.ToString();
            Message = transaction.StatusReason;
            Amount = transaction.Amount / 100M;
            GatewayFee = transaction.Cost / 100M;
            PostbackUrl = transaction.PostbackUrl;

            BoletoUrl = transaction.BoletoUrl;
            BoletoBarCode = transaction.BoletoBarcode;
            BoletoExpirationDate = transaction.BoletoExpirationDate;

            Installments = transaction.Installments ?? 1;
        }

        public TransactionResponse(TransactionCustom transaction, IList<IOrder> orders)
        {
            TransactionId = transaction.Id;
            Status = transaction.Status.ToString();
            Message = transaction.RefuseReason ?? transaction.StatusReason;
            Amount = transaction.Amount / 100M;
            GatewayFee = transaction.Cost / 100M;
            PostbackUrl = transaction.PostbackUrl;

            BoletoUrl = transaction.BoletoUrl;
            BoletoBarCode = transaction.BoletoBarcode;
            BoletoExpirationDate = transaction.BoletoExpirationDate;

            Installments = transaction.Installments ?? 1;

            if (transaction.SplitRules != null && transaction.SplitRules.Length > 0)
            {
                foreach (var splitRule in transaction.SplitRules)
                {
                    if (splitRule.Recipient.Id == PagarMeConfig.StartupRecipient)
                    {
                        StartupFee = splitRule.Amount / 100M;
                        StartupSplitId = splitRule.Id;
                    }
                    else
                    {
                        orders?.SetSplitId(splitRule);
                    }
                }
            }

            try //Attempt to list all payables and add their fees to GatewayFee
            {
                var payables = new PagarMeClient().GetService().Payables.FindAll(new Payable { TransactionId = int.Parse(TransactionId) });
                foreach (var p in payables)
                {
                    GatewayFee += p.Fee / 100M;
                }
            }
            catch { }
        }

        public bool IsPaid() => "paid".Equals(Status, StringComparison.OrdinalIgnoreCase) || "authorized".Equals(Status, StringComparison.OrdinalIgnoreCase);

        public bool IsPendingPayment() => "waitingpayment".Equals(Status, StringComparison.OrdinalIgnoreCase) || "processing".Equals(Status, StringComparison.OrdinalIgnoreCase);

        public bool IsPaymentCancelledOrRejected() => "refused".Equals(Status, StringComparison.OrdinalIgnoreCase);

        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            RefundedAmount = refundedAmount;
            RefundedStartupFee = refundedStartupFee;
            RefundedGatewayFee = refundedGatewayFee;
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            throw new InvalidOperationException("TransactionResponse class 'SetValuesForMultiPayment' does not have multipayments.");
        }
    }
}