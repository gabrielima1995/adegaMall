using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.Disputes.Messages;
using V2.Application.Modules.Disputes.Messages.Specs;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class TransferPaymentOnOrderCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IBackgroundWorker backgroundWorker;
        private readonly IScheduledTransferRepository scheduledTransferRepository;
        private readonly IPersonRepository personRepository;

        public TransferPaymentOnOrderCompleted(
            IBackgroundWorker backgroundWorker,
            IScheduledTransferRepository scheduledTransferRepository,
            IPersonRepository personRepository)
        {
            this.backgroundWorker = backgroundWorker;
            this.scheduledTransferRepository = scheduledTransferRepository;
            this.personRepository = personRepository;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (!MaaSConfig.Current.Payment.Common.EnableTransactionWarranty || domainEvent.NewStatus != OrderStatus.Completed)
                return;

            var transferResult = TryTransfer(domainEvent);
            if (transferResult.IsFailure)
                backgroundWorker.Enqueue(() => TryTransfer(domainEvent));
        }

        public OperationResult TryTransfer(OrderStatusChangedEvent domainEvent)
        {
            var transfers = scheduledTransferRepository.List().Where(transfer =>
                transfer.OrderId == domainEvent.Order.OrderId &&
                transfer.SellerId == domainEvent.Order.Seller.Id).ToList();

            foreach (var transfer in transfers)
            {
                if (!transfer.Canceled)
                {
                    transfer.Authorize();
                    scheduledTransferRepository.Update(transfer);
                }
            }

            scheduledTransferRepository.SaveChanges();

            if (transfers.Count() == 0)
                return OperationResult.Fail("No scheduled transfers found");

            NotifySellerThatPaymentIsAuthorized(domainEvent);
            return OperationResult.Success;
        }

        private void NotifySellerThatPaymentIsAuthorized(OrderStatusChangedEvent domainEvent)
        {
            var result = NotifySellerThatPaymentIsAuthorizedInternal(domainEvent);
            if (result.IsFailure)
                backgroundWorker.Enqueue(() => NotifySellerThatPaymentIsAuthorized(domainEvent));
        }

        public OperationResult NotifySellerThatPaymentIsAuthorizedInternal(OrderStatusChangedEvent domainEvent)
        {
            var buyer = personRepository.GetById(domainEvent.Order.Buyer.Id);
            var seller = personRepository.GetById(domainEvent.Order.Seller.Id);

            var result = MessagesContext.Current.GenerateAndSend<PaymentAuthorizedMessageToSellerSpecification, PaymentAuthorizedMessageModel>(
                new PaymentAuthorizedMessageModel()
                {
                    RecipientName = seller.Name,
                    OrderId = domainEvent.Order.Id,
                    BuyerName = buyer.Name,
                    SellerName = seller.AlternativeName ?? seller.Name
                },
                MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { seller.Email });

            return result;
        }
    }
}
