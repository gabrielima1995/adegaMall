using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces.Repositories;
using Domain.Modules.QuoteBillingModule.Interfaces;
using System;

namespace V2.Application.Modules.QuoteBillingModule.EventHandlers
{
    public class UpdateQuoteBillingWhenOrderStatusChanged : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IQuoteBillingRepository _quoteBillingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuoteBillingWhenOrderStatusChanged(
            IQuoteBillingRepository quoteBillingRepository,
            IUnitOfWork unitOfWork)
        {
            _quoteBillingRepository = quoteBillingRepository;
            _unitOfWork = unitOfWork;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (MaaSConfig.Current.Quote.QuoteBilling.Enabled == false)
                return;

            if (domainEvent.NewStatus.NotIn(OrderStatus.CompletedPayment, OrderStatus.Canceled, OrderStatus.Rejected))
                return;

            var billing = _quoteBillingRepository.GetForOrder(domainEvent.Order.Id);

            if (billing == null)
                return;

            var changed = false;

            if (domainEvent.NewStatus == OrderStatus.CompletedPayment)
            {
                billing.MarkPaid();
                changed = true;
            }

            else if (domainEvent.NewStatus.In(OrderStatus.Canceled, OrderStatus.Rejected))
            {
                billing.MarkRefunded();
                changed = true;
            }

            if (changed)
            {
                _quoteBillingRepository.Update(billing);
                _unitOfWork.Save();
            }
        }
    }
}
