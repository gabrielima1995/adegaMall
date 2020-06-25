using PaymentIntegrator.Core.Payments;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Dados do objeto Transaction no postback enviado pelo Pagar.me.
    /// </summary>
    public class TransactionPostbackData
    {
        public string id { get; set; }

        public string status { get; set; }

        public string payment_method { get; set; }

        public int amount { get; set; }

        public TransactionPostbackData()
        {
        }

        public TransactionPostbackData(ITransaction transaction)
        {
            id = transaction.TransactionId;
            status = transaction.Status;
            amount = (int)(transaction.Amount * 100);
        }
    }
}