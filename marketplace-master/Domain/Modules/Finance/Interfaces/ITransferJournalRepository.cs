using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Modules.Finance.Interfaces
{
    public interface ITransferJournalRepository
    {
        OperationResult AddRange(IEnumerable<TransferJournalEntry> entry);

        IEnumerable<TransferJournalEntry> ListPendingTransfersForOrder(Guid orderId);

        IEnumerable<TransferJournalEntry> ListPendingTransfersToDate(DateTime endDate);

        OperationResult Update(TransferJournalEntry entry);
    }
}
