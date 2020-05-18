using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;

namespace Infrastructure.Shipping.Adapters
{
    public abstract class ShippingMethodAdapterBase
    {
        public OperationResult ChangeStatus(Order order, OrderStatus newStatus, Person person = null, string additionalInfo = null)
        {
            order.ChangeStatus(newStatus, person, additionalInfo);
            return OperationResult.Success;
        }

        public abstract List<OrderStatus> ListAvailableStatuses(OrderStatus currentStatus);
    }
}