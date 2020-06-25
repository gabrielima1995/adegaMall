using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.Orders;
using Framework;
using System;
using System.Linq;

namespace Application.Modules.Products.Orders
{
    public class RefundOrderOnOrderCancelledOrRejected : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IBackgroundWorker _backgroundWorker;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderService_Obsolete _orderService;

        public RefundOrderOnOrderCancelledOrRejected(
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
            // somente se pedido cancelado ou rejeitado
            if (domainEvent.NewStatus == OrderStatus.Canceled || domainEvent.NewStatus == OrderStatus.Rejected)
            {
                if (!ShouldRefund(domainEvent.Order))
                    return;

                var order = domainEvent.Order;
                var config = MaaSConfig.Current;
                var refundResult = TryRefund(order.Id);

                if (!refundResult.OK)
                    _backgroundWorker.Enqueue<RefundOrderOnOrderCancelledOrRejected>(x => TryRefund(order.Id));
            }
        }

        public OperationResult TryRefund(Guid orderId)
        {
            return _orderService.CancelOrReject(orderId);
        }

        private bool ShouldRefund(Order order)
        {
            if (order.Histories == null || !order.Histories.Any())
                order = _unitOfWork.OrderRepository.GetDetails(order.Id);

            var antifraudDenied = order.Histories.Any(x => x.Status == OrderStatus.AntifraudDenied);
            var cancelled = order.Histories.Any(x => x.Status == OrderStatus.Canceled);

            if (antifraudDenied && cancelled)
            {
                if (order.Histories.Any(x => x.Status.In(
                        OrderStatus.Delivered,
                        OrderStatus.DeliveryOnTheWay)))
                    return false;
            }

            return true;
        }
    }
}