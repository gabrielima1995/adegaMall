using Application.Config;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.Disputes;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace V2.Application.Modules.Finance
{
    public class TransactionWarrantyTransferJobs
    {
        private readonly IScheduledTransferRepository scheduledTransferRepository;
        private readonly IPaymentOperations paymentOperations;
        private readonly IOrderRepository orderRepository;
        private readonly IOrderAppServiceV2 orderAppService;

        public TransactionWarrantyTransferJobs(
            IScheduledTransferRepository scheduledTransferRepository,
            IPaymentOperations paymentOperations,
            IOrderRepository orderRepository,
            IOrderAppServiceV2 orderAppService)
        {
            this.scheduledTransferRepository = scheduledTransferRepository;
            this.paymentOperations = paymentOperations;
            this.orderRepository = orderRepository;
            this.orderAppService = orderAppService;
        }

        public void AutoCloseOrders()
        {
            var ordersToClose = orderRepository
                .List(x => x.Dispute, x => x.Buyer, x => x.Seller)
                .Where(x => x.Dispute == null)
                .Where(x => x.Status != OrderStatus.Completed && x.AutoCompletionDate < DateTime.Now)
                .ToList();

            if (ordersToClose.Any())
            {
                foreach (var order in ordersToClose)
                {
                    if (StatusIsNotRight(order.Status))
                        continue;

                    ThrowIfFailure(orderAppService.CloseDisputeAndCompleteOrder(order), "Error closing dispute and completing order.");
                    ThrowIfFailure(orderRepository.Update(order), "Error updating order.");
                }

                ThrowIfFailure(orderRepository.SaveChanges(), "Error saving changes in job that automatically closes orders.");
            }
        }

        public void DoScheduledTransfersIfOrderCompleted()
        {
            var transferList = scheduledTransferRepository.ListPendingTransfers();
            var changesMade = false;
            var errors = new List<string>();

            foreach (var scheduledTransfer in transferList)
            {
                if (scheduledTransfer.GatewayTransferId != null)
                {
                    var gatewayTransfer = paymentOperations.GetTransferDetails(scheduledTransfer.GatewayTransferId);
                    if (gatewayTransfer.Result.IsTransferred)
                    {
                        scheduledTransfer.GatewayTransferConfirmed();
                        changesMade = true;
                    }
                }
                else
                {
                    var payable = paymentOperations.GetPayableDetails(scheduledTransfer.GatewayPayableId)?.Result;

                    if (payable == null || !payable.IsPaid)
                        continue;

                    if (!TransferIsAllowedNow(Guid.Parse(scheduledTransfer.OrderId)))
                        continue;

                    var transferResult = paymentOperations.CreateTransferForPayable(payable.PayableId);

                    if (transferResult.IsFailure)
                    {
                        if (!TransferErrorIsNoFunds(transferResult.Errors))
                            errors.Add($"Unable to create transfer for payable: {payable.PayableId}. " + transferResult.Errors.ToSimpleString());
                        continue;
                    }

                    var dto = transferResult.Result;
                    scheduledTransfer.SetGatewayTransferId(dto.Id, IsTransferred(dto));
                    changesMade = true;
                }

                if (changesMade)
                    scheduledTransferRepository.Update(scheduledTransfer);
            }

            if (changesMade)
                scheduledTransferRepository.SaveChanges();

            if (errors.Any())
            {
                var allErrors = string.Join(Environment.NewLine, errors);
                throw new Exception(allErrors);
            }
        }

        public void NotifyUpcomingCompletions()
        {
            var notifyOrderList = orderRepository.ListByUpcomingCompletion(MaaSConfig.Current.Payment.Common.OrderConclusionNotificationDays);

            foreach (var order in notifyOrderList)
                NotifyOrderAboutToAutoClose(order);
        }

        private bool TransferErrorIsNoFunds(ErrorDictionary errors)
        {
            var error = errors.ToSimpleString();
            return error.IndexOf("saldo insuficiente", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool StatusIsNotRight(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.CompletedPayment:
                case OrderStatus.AwaitingDelivery:
                case OrderStatus.DeliveryOnTheWay:
                case OrderStatus.Delivered:
                case OrderStatus.AwaitingPickup:
                    return false;

                default:
                    return true;
            }
        }

        private bool TransferIsAllowedNow(Guid orderId)
        {
            return orderRepository.CheckStatusCompleted_NoDispute_OrDisputeClosed(orderId);
        }

        private bool IsTransferred(TransferDto transfer)
        {
            switch (MaaSConfig.Current.Payment.ActiveGateway)
            {
                case "pagarme":
                    return transfer.Status.Equals("transferred", StringComparison.OrdinalIgnoreCase);

                case "moip":
                    return transfer.Status.Equals("released", StringComparison.OrdinalIgnoreCase);

                default:
                    throw new InvalidOperationException($"PaymentTransferJobs.IsTransferred: not implemented for gateway {MaaSConfig.Current.Payment.ActiveGateway}.");
            }
        }

        private OperationResult NotifyOrderAboutToAutoClose(Order order)
        {
            var result = MessagesContext.Current.GenerateAndSend<OrderAutoCompletionImminentMessageToBuyerSpecification, OrderAutoCompletionImminentMessageModel>(
                new OrderAutoCompletionImminentMessageModel()
                {
                    AutoCompletionDate = order.AutoCompletionDate.Value.ToShortDateString(),
                    BuyerName = order.Buyer.Name,
                    Link = MaaSConfig.Current.Environment.AppUrl + "myorders/details/" + order.Id + "/?lang=" + order.Buyer.Lang,
                    OrderId = order.OrderId,
                    SellerName = order.Seller.AlternativeName ?? order.Seller.Name,
                }, MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                new[] { order.Buyer.Email });

            return result;
        }

        private void ThrowIfFailure(OperationResult result, string additionalMessage = null)
        {
            if (result.IsFailure)
                throw new Exception(additionalMessage + " " + result.Errors.ToSimpleString());
        }
    }
}