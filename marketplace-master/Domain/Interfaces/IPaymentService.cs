using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IPaymentService
    {
        OperationResult<ICreditCardResult> CreateCreditCard(Buyer buyer, DTO.CreditCard creditCard);

        OperationResult<ITransaction> GetTransaction(string transactionId);

        OperationResult<ITransaction> Pay(IEnumerable<Order> orders, Buyer buyer, PaymentOptions paymentOptions);

        OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund);

        OperationResult RegisterAsCustomer(Buyer buyer, Address address);

        OperationResult SetAutoTransfer(string id, bool enabled);

        OperationResult ScheduleTransfersToSellersForWhenFundsAreAvailable(Order order);
    }
}
