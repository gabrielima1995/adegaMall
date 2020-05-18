using System;
using System.Linq;

namespace PaymentIntegrator.Core.Payments
{
    public class PayableDto
    {
        public DateTime OriginalPaymentDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string RecipientId { get; set; }
        public string PayableId { get; set; }
        public string BulkAnticipationId { get; set; }
        public string SplitRuleId { get; set; }
        public int TransactionId { get; set; }
        public int Installment { get; set; }
        public int AnticipationFee { get; set; }
        public int Fee { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string PayableType { get; set; }
        public string PaymentMethod { get; set; }

        public bool IsPaid
        {
            get
            {
                var possibleStatuses = new[] { /*pagarme*/ "paid", /*wirecard*/ "held", "hold_pending" };
                return possibleStatuses.Any(statusToTest => statusToTest.Equals(Status, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
