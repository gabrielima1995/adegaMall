using Domain.Modules.Finance;
using Framework;
using System;
using System.Collections.Generic;
using V2.Application.Modules.Finance.Model;

namespace V2.Application.Modules.Finance.Interfaces
{
    public interface IPaymentGateway
    {
        IEnumerable<ReceivableDto> ListReceivablesByTransactionId(string transactionId);
        IEnumerable<TransferDto> ListCompletedTransfersForOrder(Guid orderId);
        OperationResult<string> DoTransfer(TransferJournalEntry data);
    }
}
