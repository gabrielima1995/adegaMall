using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class OrderTransactionRepository : EfCrudRepository<OrderTransaction>, IOrderTransactionRepository
    {
        public OrderTransactionRepository(ScaffoldContext context) : base(context)
        {
        }

        public OrderTransaction GetByGatewayId(string gatewayId)
        {
            return CacheHelper.Get(Context.Set<OrderTransaction>().Where(x => x.TransactionId == gatewayId));
        }
    }
}
