using Application.Config;
using Domain.Entities.StoreModule;
using PaymentIntegrator.Core.Payments;
using System;

namespace Domain.Entities
{
    public class ScheduledTransfer : BaseEntity<Guid>
    {
        private ScheduledTransfer()
        {
        }

        private ScheduledTransfer(
            string orderId, string buyerId, string sellerId, string gatewayPayableId, string gatewayRecipientId,
            int installments, int currentInstallment, DateTime dueDate, DateTime originalDueDate,
            decimal transferAmount, decimal feesAmount, decimal fullAmount)
        {
            Id = Guid.NewGuid();

            OrderId = orderId;
            BuyerId = buyerId;
            SellerId = sellerId;
            GatewayPayableId = gatewayPayableId;
            GatewayRecipientId = gatewayRecipientId;

            Installments = installments;
            CurrentInstallment = currentInstallment;
            DueDate = dueDate;
            OriginalDueDate = originalDueDate;

            TransferAmount = transferAmount;
            FeesAmount = feesAmount;
            FullAmount = fullAmount;

            Anticipated = false;
            TransferAuthorized = false;
            Transferred = false;
            Canceled = false;
        }

        public static ScheduledTransfer FromOrderAndPayable(Order order, PayableDto payable)
        {
            return new ScheduledTransfer(
                order.OrderId, order.Buyer.Id, order.Seller.Id, payable.PayableId, order.Seller.SupplierGatewayId,
                order.Transaction.Installments, payable.Installment, payable.PaymentDate, payable.OriginalPaymentDate,
                payable.Amount - payable.Fee, payable.Fee, payable.Amount);
        }

        public string OrderId { get; private set; }
        public string BuyerId { get; private set; }
        public string SellerId { get; private set; }

        public decimal FullAmount { get; private set; }
        public decimal FeesAmount { get; private set; }
        public decimal TransferAmount { get; private set; }

        public DateTime DueDate { get; private set; }
        public DateTime OriginalDueDate { get; private set; }
        public DateTime? CompletionDate { get; private set; }
        public DateTime? CancelationDate { get; private set; }

        public bool Anticipated { get; private set; }
        public bool TransferAuthorized { get; private set; }
        public bool Transferred { get; private set; }
        public bool Canceled { get; private set; }

        public string GatewayRecipientId { get; private set; }
        public string GatewayPayableId { get; private set; }
        public string GatewayTransferId { get; private set; }

        public int Installments { get; private set; }
        public int CurrentInstallment { get; private set; }

        public void Authorize()
        {
            if (Canceled)
                throw new InvalidOperationException("Can not authorize a cancelled transfer.");

            TransferAuthorized = true;
        }

        public void GatewayTransferConfirmed()
        {
            if (GatewayTransferId == null)
                throw new InvalidOperationException("Can not mark as transferred without specifying a gateway id.");

            Transferred = true;
        }

        public void SetGatewayTransferId(string gatewayTransferId, bool isTransferred)
        {
            if (string.IsNullOrEmpty(gatewayTransferId))
                throw new ArgumentNullException(nameof(gatewayTransferId));

            GatewayTransferId = gatewayTransferId;
            Transferred = isTransferred;
        }
    }
}
