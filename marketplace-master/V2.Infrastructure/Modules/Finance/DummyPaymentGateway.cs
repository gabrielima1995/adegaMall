using Domain.Modules.Finance;
using Framework;
using System;
using System.Collections.Generic;
using V2.Application.Modules.Finance.Interfaces;
using V2.Application.Modules.Finance.Model;

namespace V2.Infrastructure.Modules.Finance
{
    public class DummyPaymentGateway : IPaymentGateway
    {
        private const string Message = "No payment gateway enabled";

        public OperationResult<string> DoTransfer(TransferJournalEntry data)
        {
            throw new NotImplementedException(Message);
        }

        public IEnumerable<TransferDto> ListCompletedTransfersForOrder(Guid orderId)
        {
            throw new NotImplementedException(Message);
        }

        public IEnumerable<ReceivableDto> ListReceivablesByTransactionId(string transactionId)
        {
            throw new NotImplementedException(Message);
        }
    }
}
