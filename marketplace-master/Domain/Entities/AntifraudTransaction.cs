using System;

namespace Domain.Entities
{
    public class AntifraudTransaction : BaseEntity<Guid>
    {
        public string TransactionId { get; private set; }

        public string RequestContent { get; private set; }

        public string Status { get; private set; }

        public string Message { get; private set; }

        public string Content { get; private set; }

        public AntifraudTransaction()
        {
            // Parameterless constructor required by EF.
        }

        public AntifraudTransaction(string transactionId, string requestContent, string status, string message, string content) : base()
        {
            Id = Guid.NewGuid();
            TransactionId = transactionId;
            RequestContent = requestContent;
            Status = status;
            Message = message;
            Content = content;
        }
    }
}
