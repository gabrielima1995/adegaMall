using Application.Config;
using Domain.Core.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Modules.Finance;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using V2.Application.Modules.Finance.Interfaces;

namespace V2.Application.Modules.Finance
{
    public class DoTransfersRecurrentJob
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUnitOfWork _unitOfWork;

        public DoTransfersRecurrentJob(
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork)
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
        }

        public void Run()
        {
            if (!MaaSConfig.Current.Payment.Enabled || !MaaSConfig.Current.Finance.TransfersEnabled)
                return;

            var pendingTransfers = GetPendingTransfers();
            var doubleCheck = DoubleCheckAgainstGateway(pendingTransfers);

            ProcessActuallyPending(doubleCheck.ActuallyPending);
            ProcessAlreadyCompleted(doubleCheck.AlreadyCompleted);
            ProcessNotFound(doubleCheck.NotFound);

            var saveResult = _unitOfWork.Save();
            if (!saveResult.OK) throw new OperationResultException(saveResult);
        }

        private void DoTransferAndMarkCompleted(TransferJournalEntry entry)
        {
            OperationResult result = null;

            for (var i = 0; i < 3; i++)
            {
                result = _paymentGateway.DoTransfer(entry);
                if (result.OK) break;
                else Thread.Sleep(500);
            }

            if (result.OK) entry.MarkCompleted(DateTime.UtcNow);
            else entry.AppendNote($"Failed transfer attempt at {DateTime.UtcNow} for reason '{result.Errors.ToSimpleString()}'.");
        }

        private DoubleCheckResults DoubleCheckAgainstGateway(IEnumerable<TransferJournalEntry> entries)
        {
            var groups = entries.GroupBy(x => x.GatewayTransactionId).ToList();

            var actuallyPending = new List<TransferJournalEntry>(entries.Count());
            var notFound = new List<TransferJournalEntry>(entries.Count());
            var alreadyCompleted = new List<TransferJournalEntry>(entries.Count());

            foreach (var g in groups)
            {
                var transfersForOrder = g.ToList();
                var receivables = _paymentGateway.ListReceivablesByTransactionId(g.Key);
                var completedTransfers = _paymentGateway.ListCompletedTransfersForOrder(g.First().OrderId);

                var _notFound = transfersForOrder.Where(transfer => !receivables.Any(receivable =>
                    receivable.Id == transfer.GatewayReceivableId &&
                    receivable.IsAmountAvailableForTransfer));

                if (_notFound.Any())
                {
                    notFound.AddRange(_notFound);
                    transfersForOrder = transfersForOrder.Except(_notFound).ToList();
                }

                var _completed = transfersForOrder.Where(transfer => completedTransfers.Any(completed =>
                    completed.ComparisonKey == transfer.KeyForGatewayComparison))
                        .ToList();

                if (_completed.Any())
                {
                    alreadyCompleted.AddRange(_completed);
                    transfersForOrder = transfersForOrder.Except(_completed).ToList();
                }

                if (transfersForOrder.Any())
                    actuallyPending.AddRange(transfersForOrder);
            }

            return new DoubleCheckResults(actuallyPending, notFound, alreadyCompleted);
        }

        private IEnumerable<TransferJournalEntry> GetPendingTransfers()
        {
            return _unitOfWork.TransferJournalRepository.ListPendingTransfersToDate(DateTime.UtcNow);
        }

        private void ProcessActuallyPending(IEnumerable<TransferJournalEntry> entries)
        {
            if (!entries.Any()) return;

            foreach (var t in entries)
            {
                DoTransferAndMarkCompleted(t);
                var updateResult = _unitOfWork.TransferJournalRepository.Update(t);
                if (!updateResult.OK) throw new OperationResultException(updateResult);
            }
        }

        private void ProcessAlreadyCompleted(IEnumerable<TransferJournalEntry> entries)
        {
            if (!entries.Any()) return;

            foreach (var t in entries)
            {
                t.MarkCompleted(DateTime.Now);
                var updateResult = _unitOfWork.TransferJournalRepository.Update(t);
                if (!updateResult.OK) throw new OperationResultException(updateResult);
            }
        }

        private void ProcessNotFound(IEnumerable<TransferJournalEntry> entries)
        {
            if (!entries.Any()) return;

            foreach (var t in entries)
            {
                t.Cancel($"A matching receivable was not found in the payment gateway when double-checking before completing the transfer.");
                var updateResult = _unitOfWork.TransferJournalRepository.Update(t);
                if (!updateResult.OK) throw new OperationResultException(updateResult);
            }
        }

        private class DoubleCheckResults
        {
            public IEnumerable<TransferJournalEntry> ActuallyPending { get; }
            public IEnumerable<TransferJournalEntry> NotFound { get; }
            public IEnumerable<TransferJournalEntry> AlreadyCompleted { get; }

            public DoubleCheckResults(
                IEnumerable<TransferJournalEntry> actuallyPending,
                IEnumerable<TransferJournalEntry> notFound,
                IEnumerable<TransferJournalEntry> alreadyCompleted)
            {
                ActuallyPending = actuallyPending;
                NotFound = notFound;
                AlreadyCompleted = alreadyCompleted;
            }
        }
    }
}
