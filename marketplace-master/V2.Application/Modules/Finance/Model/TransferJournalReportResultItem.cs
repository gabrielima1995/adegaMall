using Domain.Entities;
using Domain.Modules.Finance;
using System;

namespace V2.Application.Modules.Finance.Model
{
    public class TransferJournalReportResultItem
    {
        public Guid Id { get; set; }

        public TransferJournalEntryType Type { get; set; }

        public Guid OrderId { get; set; }

        public Buyer Buyer { get; set; }

        public Seller Seller { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal CommissionPct { get; set; }

        public short InstallmentsCount { get; set; }

        public short InstallmentNumber { get; set; }

        public decimal InstallmentAmount { get; set; }

        public decimal InstallmentCommissionAmount { get; set; }

        public decimal TransferAmount { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public string GatewayReceiverId { get; set; }

        public string GatewayTransactionId { get; set; }

        public string AdditionalNotes { get; set; }
    }
}
