using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;

namespace Domain.Services.StoreModule
{
    public class OrderTransactionService
    {
        private readonly IOrderTransactionRepository _orderTransactionRepository;

        public OrderTransactionService(IOrderTransactionRepository orderTransactionRepository)
        {
            _orderTransactionRepository = orderTransactionRepository;
        }

        public OrderTransaction GetByGatewayId(string gatewayTransactionId)
        {
            return _orderTransactionRepository.GetByGatewayId(gatewayTransactionId);
        }
    }
}
