using PaymentIntegrator.Core.Payments;
using System;

namespace PaymentIntegrator.Moip.Orders
{
    public static class MoipOrder
    {
        public static object CreateRefund(IOrder order, ITransaction transaction, decimal totalRefund)
        {
            var obj = new
            {
                amount = totalRefund.ToCents()
            };

            return obj;
        }

        public static PayableDto CreateEscrow(dynamic orderDetails, string recipientId)
        {
            DateTime originalPayDate = DateTime.UtcNow;
            try
            {
                originalPayDate = (DateTime)(orderDetails.entries[0].scheduledFor);
            }
            catch { }

            var result = new PayableDto()
            {
                Amount = (int)orderDetails.amount.paid,
                PaymentDate = originalPayDate,
                OriginalPaymentDate = originalPayDate,
                Status = (string)orderDetails.escrows[0].status,
                Installment = 1,
                RecipientId = recipientId,
                SplitRuleId = (string)orderDetails.escrows[0].id,
                PayableId = (string)orderDetails.id,
                Fee = (int)orderDetails.amount.fees,
            };

            return result;
        }
    }
}