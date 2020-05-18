using Application.Config;
using Domain.Core.Exceptions;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces.Repositories;
using System;
using System.Linq;

namespace V2.Application.Modules.Finance.EventHandlers
{
    public class CancelTransferToSellerOnOrderCancelled : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelTransferToSellerOnOrderCancelled(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            // somente se antifraude foi negada, pedido cancelado ou rejeitado
            if (domainEvent.NewStatus.In(
                    OrderStatus.AntifraudDenied,
                    OrderStatus.Canceled,
                    OrderStatus.Rejected))
            {

                if (!MaaSConfig.Current.Payment.Enabled || !MaaSConfig.Current.Finance.TransfersEnabled)
                    return;

                var transfers =
                    _unitOfWork.TransferJournalRepository.ListPendingTransfersForOrder(domainEvent.Order.Id);
                if (!transfers.Any()) return;

                var reason =
                    $"Transfer has been cancelled because Order Status changed to '{domainEvent.NewStatus.ToString()}'.";

                foreach (var t in transfers)
                {
                    t.Cancel(reason);
                    var updateResult = _unitOfWork.TransferJournalRepository.Update(t);
                    if (!updateResult.OK) throw new OperationResultException(updateResult);
                }

                var saveResult = _unitOfWork.Save();
                if (!saveResult.OK) throw new OperationResultException(saveResult);
            }
        }
    }
}
