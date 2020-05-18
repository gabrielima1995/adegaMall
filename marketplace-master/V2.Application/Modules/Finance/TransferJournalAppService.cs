using Domain.Interfaces.Repositories;
using Domain.Modules.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.Finance.Interfaces;

namespace V2.Application.Modules.Finance
{
    public class TransferJournalAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentGateway _paymentGateway;

        public TransferJournalAppService(
            IOrderRepository orderRepository,
            IPaymentGateway paymentGateway)
        {
            _orderRepository = orderRepository;
            _paymentGateway = paymentGateway;
        }

        public IEnumerable<TransferJournalEntry> BuildTransferEntries(Guid orderId)
        {
            var order = _orderRepository.GetDetails(orderId);
            var list = new List<TransferJournalEntry>();

            if (order.Transaction != null)
            {
                var receivables = _paymentGateway.ListReceivablesByTransactionId(order.Transaction.TransactionId);

                if (receivables.Any())
                {
                    var comissionPct = order.Transaction.Amount > 0M ? order.Transaction.StartupFee / order.Transaction.Amount : 0M;
                    comissionPct = comissionPct.RoundTo2Places();

                    var commissionAmountPerInstallment = (order.Transaction.StartupFee / order.Transaction.Installments).RoundTo2Places();
                    var commissionRemainder = order.Transaction.StartupFee - (commissionAmountPerInstallment * order.Transaction.Installments);
                    var commissionFirstInstallment = commissionAmountPerInstallment + commissionRemainder;

                    foreach (var r in receivables)
                    {
                        var entry = TransferJournalEntry.Create(
                            Guid.NewGuid(),
                            TransferJournalEntryType.NewOrder,
                            orderId,
                            order.Buyer.Id,
                            order.Seller.Id,
                            order.PaymentMethodString,
                            order.CreatedAt,
                            order.Transaction.Amount,
                            comissionPct,
                            (short)order.Transaction.Installments,
                            r.InstallmentNumber,
                            r.Amount,
                            r.InstallmentNumber == 1 ? commissionFirstInstallment : commissionAmountPerInstallment,
                            r.TransferDueDate,
                            r.Id,
                            order.Seller.SupplierGatewayId,
                            order.Transaction.TransactionId);

                        list.Add(entry);
                    }
                }
            }

            return list;
        }
    }
}
