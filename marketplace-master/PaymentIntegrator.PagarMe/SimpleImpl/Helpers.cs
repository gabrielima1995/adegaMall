using PagarMe;

namespace PaymentIntegrator.PagarMe.SimpleImpl
{
    public static class Helpers
    {
        public static TransactionStatus ParseTransactionStatus(string status)
        {
            return
                status == "processing" ? TransactionStatus.Processing :
                status == "authorized" ? TransactionStatus.Authorized :
                status == "paid" ? TransactionStatus.Paid :
                status == "refunded" ? TransactionStatus.Refunded :
                status == "waiting_payment" ? TransactionStatus.WaitingPayment :
                status == "pending_refund" ? TransactionStatus.PendingRefund :
                status == "refused" ? TransactionStatus.Refused :
                TransactionStatus.None;
        }
    }
}
