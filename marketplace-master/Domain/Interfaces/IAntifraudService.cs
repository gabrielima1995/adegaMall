using Domain.DTO;
using Domain.Entities.StoreModule;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IAntifraudService
    {
        OperationResult CancelAnalisys(string transactionId);

        OperationResult<AntifraudOrderStatus> GetOrderStatus(Guid orderId);

        OperationResult<AntifraudOrderStatus> RunAnalisys(string sessionId, IEnumerable<Order> orders, PaymentOptions paymentOptions);
    }
}
