using Domain.Entities.StoreModule;
using Framework;

namespace V2.Application.Modules.Disputes
{
    public interface IOrderAppServiceV2
    {
        OperationResult<Order> CloseDisputeAndCompleteOrder(Order order);
    }
}
