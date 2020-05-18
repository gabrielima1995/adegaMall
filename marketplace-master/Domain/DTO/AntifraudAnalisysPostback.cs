using System;

namespace Domain.DTO
{
    public class AntifraudAnalisysPostback
    {
        /// <summary>
        /// Id de transação criado pelo gateway de antifraude (não confundir com a transação de pagamento).
        /// </summary>
        public string TransactionId { get; private set; }

        public Guid OrderId { get; private set; }

        public string Status { get; private set; }

        public string Message { get; private set; }

        public AntifraudAnalisysPostback()
        {
        }

        public AntifraudAnalisysPostback(string transactionId, Guid orderId, string status, string message)
        {
            TransactionId = transactionId;
            OrderId = orderId;
            Status = status;
            Message = message;
        }
    }
}
