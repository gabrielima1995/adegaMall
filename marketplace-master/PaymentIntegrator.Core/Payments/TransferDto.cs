using System;

namespace PaymentIntegrator.Core.Payments
{
    public class TransferDto
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string SourceType { get; set; }
        public string SourceId { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public int Fee { get; set; }
        public DateTime? FundingDate { get; set; }
        public DateTime? FundingEstimatedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string TransactionId { get; set; }
        public TransferToBankAccount BankAccount { get; set; } = new TransferToBankAccount();

        public string ErrorMessage { get; set; }

        public bool IsTransferred =>
            "transferred".Equals(Status, StringComparison.OrdinalIgnoreCase) ||
            "completed".Equals(Status, StringComparison.OrdinalIgnoreCase);

        public class TransferToBankAccount
        {
            public string Id { get; set; }
            public string BankCode { get; set; }
            public string Branch { get; set; }
            public string BranchCheckNumber { get; set; }
            public string Account { get; set; }
            public string AccountCheckNumber { get; set; }
            public string AccountType { get; set; }
            public string AccountableDocumentType { get; set; }
            public string AccountableDocumentNumber { get; set; }
            public string AccountableName { get; set; }
        }
    }
}