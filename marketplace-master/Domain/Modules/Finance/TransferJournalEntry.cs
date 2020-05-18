using Domain.Entities;
using Domain.Events.Finance;
using Framework;
using System;
using System.Text;

namespace Domain.Modules.Finance
{
    public class TransferJournalEntry : BaseEntity<Guid>
    {
        public TransferJournalEntryType Type { get; private set; }

        public Guid OrderId { get; private set; }

        public string BuyerId { get; private set; }

        public string SellerId { get; private set; }

        public string PaymentMethod { get; private set; }

        public DateTime OrderDate { get; private set; }

        public decimal OrderTotal { get; private set; }

        public decimal CommissionPct { get; private set; }

        public short InstallmentsCount { get; private set; }

        public short InstallmentNumber { get; private set; }

        public decimal InstallmentAmount { get; private set; }

        public decimal InstallmentCommissionAmount { get; private set; }

        public decimal TransferAmount { get; private set; }

        public DateTime DueDate { get; private set; }

        public bool Completed { get; private set; }

        public DateTime? CompletedDate { get; private set; }

        public string GatewayReceivableId { get; private set; }

        public string GatewayReceiverId { get; private set; }

        public string GatewayTransactionId { get; private set; }

        public bool Cancelled { get; private set; }

        public DateTime? CancelledDate { get; private set; }

        public string AdditionalNotes { get; private set; }

        public string KeyForGatewayComparison => $"{OrderId}_{InstallmentNumber}_{GatewayReceivableId}";

        public TransferJournalEntry() { }

        public void AppendNote(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var sb = new StringBuilder(AdditionalNotes);
                sb.AppendLine(text);
            }
        }

        public void Cancel(string reason)
        {
            Guard.Ensure.NotEmpty(reason, nameof(reason), "A reason must be provided when cancelling a transfer.");
            Guard.Ensure.IsFalse(Completed, nameof(Completed), $"Cannot cancel a completed transfer ('{Id}').");

            Cancelled = true;
            CancelledDate = DateTime.UtcNow;
            AppendNote(reason);
        }

        public static TransferJournalEntry Create(
            Guid id, TransferJournalEntryType type, Guid orderId,
            string buyerId, string sellerId, string paymentMethod,
            DateTime orderDate, decimal orderTotal, decimal commissionPct,
            short installmentsCount, short installmentNumber,
            decimal installmentAmount, decimal installmentCommissionAmount,
            DateTime dueDate, string gatewayReceivableId, string gatewayReceiverId, string gatewayTransactionId)
        {
            Guard.Ensure.NotEmpty(id, nameof(id));
            Guard.Ensure.NotEmpty(orderId, nameof(orderId));
            Guard.Ensure.NotEmpty(buyerId, nameof(buyerId));
            Guard.Ensure.NotEmpty(sellerId, nameof(sellerId));
            Guard.Ensure.NotEmpty(paymentMethod, nameof(paymentMethod));
            Guard.Ensure.GreaterOrEqual(installmentsCount, 1, nameof(installmentsCount), "Number of installments must be at least 1.");
            Guard.Ensure.BetweenInclusive(installmentNumber, 1, installmentsCount, nameof(installmentNumber), $"Installment number must range from 1 to {installmentsCount}, which is the installments count for this order.");
            Guard.Ensure.LowerOrEqual(installmentCommissionAmount, installmentAmount, nameof(installmentCommissionAmount), "Commission amount cannot be greater than the installment amount.");
            Guard.Ensure.GreaterOrEqual(dueDate, orderDate, nameof(dueDate), "Cannot have DueDate be less than OrderDate.");
            Guard.Ensure.NotEmpty(gatewayReceivableId, nameof(gatewayReceivableId));
            Guard.Ensure.NotEmpty(gatewayReceiverId, nameof(gatewayReceiverId));
            Guard.Ensure.NotEmpty(gatewayTransactionId, nameof(gatewayTransactionId));

            var entry = new TransferJournalEntry()
            {
                Id = id,
                Type = type,
                OrderId = orderId,
                BuyerId = buyerId,
                SellerId = sellerId,
                PaymentMethod = paymentMethod,
                OrderDate = orderDate,
                OrderTotal = orderTotal,
                CommissionPct = commissionPct,
                InstallmentsCount = installmentsCount,
                InstallmentNumber = installmentNumber,
                InstallmentAmount = installmentAmount,
                InstallmentCommissionAmount = installmentCommissionAmount,
                TransferAmount = installmentAmount - installmentCommissionAmount,
                DueDate = dueDate.GetBusinessDay(),
                GatewayReceivableId = gatewayReceivableId,
                GatewayReceiverId = gatewayReceiverId,
                GatewayTransactionId = gatewayTransactionId,
                Cancelled = false,
                CancelledDate = null,
                Completed = false,
                CompletedDate = null,
                AdditionalNotes = null,
                CreatedAt = DateTime.UtcNow
            };

            entry.AddDomainEvent(new TransferJournalEntryAddedEvent(entry));
            return entry;
        }

        public void MarkCompleted(DateTime completedDate)
        {
            Guard.Ensure.IsFalse(Cancelled, nameof(Cancelled), $"Cannot complete a cancelled transfer ('{Id}').");

            Completed = true;
            CompletedDate = DateTime.UtcNow;
        }
    }
}
