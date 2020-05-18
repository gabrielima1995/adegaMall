using Application.Config;
using Domain.Core.Exceptions;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Modules.Finance;

namespace V2.Application.Modules.Finance.EventHandlers
{
    public class ScheduleTransferToSellerOnPaymentCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly TransferJournalAppService _transferJournalAppService;
        private readonly TransferJournalService _transferJournalService;

        public ScheduleTransferToSellerOnPaymentCompleted(
            TransferJournalAppService transferJournalAppService,
            TransferJournalService transferJournalService)
        {
            _transferJournalAppService = transferJournalAppService;
            _transferJournalService = transferJournalService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus != OrderStatus.CompletedPayment)
                return;

            if (!MaaSConfig.Current.Payment.Enabled || !MaaSConfig.Current.Finance.TransfersEnabled)
                return;

            var entries = _transferJournalAppService.BuildTransferEntries(domainEvent.Order.Id);
            var scheduleResult = _transferJournalService.ScheduleTransfers(entries);

            if (!scheduleResult.OK)
                throw new OperationResultException(scheduleResult);
        }
    }
}
