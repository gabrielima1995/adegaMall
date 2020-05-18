using System;

namespace V2.Application.Modules.Finance.Model
{
    public class TransferDto
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public string ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        public string ComparisonKey { get; set; }
    }
}
