using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.Orders;
using Framework;
using System;

namespace Application.Modules.Products.Orders
{
    public class NewTransactionRentalOnOrder : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IBackgroundWorker _backgroundWorker;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderService_Obsolete _orderService;

        public NewTransactionRentalOnOrder(
            IBackgroundWorker backgroundWorker,
            IUnitOfWork unitOfWork,
            OrderService_Obsolete orderService)
        {
            _backgroundWorker = backgroundWorker;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            // somente se for locação entregue ou devolvida ou rejeitada
            if (domainEvent.Order.IsRental &&
                domainEvent.NewStatus.In(OrderStatus.Delivered, OrderStatus.Returned, OrderStatus.Rejected))
            {
                _backgroundWorker.Schedule<NewTransactionRentalOnOrder>(
                    x => x.TryTransactionRental(domainEvent.Order.Id, domainEvent.NewStatus),
                    TimeSpan.FromSeconds(30));
            }
        }

        public OperationResult TryTransactionRental(Guid orderId, OrderStatus orderStatus)
        {
            return _orderService.NewStatusRental(orderId, orderStatus);
        }
    }
}