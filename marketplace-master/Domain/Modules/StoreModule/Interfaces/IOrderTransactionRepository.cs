using Domain.Entities.StoreModule;

namespace Domain.Interfaces.Repositories
{
    public interface IOrderTransactionRepository : ICrudRepository<OrderTransaction>
    {
        OrderTransaction GetByGatewayId(string gatewayId);
    }
}
