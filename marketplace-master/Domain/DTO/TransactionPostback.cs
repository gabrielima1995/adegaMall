namespace Domain.DTO
{
    public class TransactionPostback
    {
        public string TransactionId { get; set; }

        public string PostbackData { get; set; }

        public TransactionPostback()
        {
        }

        public TransactionPostback(string transactionId, string postbackData)
        {
            TransactionId = transactionId;
            PostbackData = postbackData;
        }
    }
}
