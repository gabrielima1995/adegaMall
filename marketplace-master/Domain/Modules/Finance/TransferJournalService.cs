using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Modules.Finance
{
    public class TransferJournalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferJournalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OperationResult CancelTransfersForOrder(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public OperationResult ScheduleTransfers(IEnumerable<TransferJournalEntry> transfers)
        {
            Guard.Ensure.NotEmpty(transfers, nameof(transfers));

            var addResult = _unitOfWork.TransferJournalRepository.AddRange(transfers);
            if (!addResult.OK) return addResult;

            var saveResult = _unitOfWork.Save();
            if (!saveResult.OK) return saveResult;

            return OperationResult.Success;
        }
    }
}
