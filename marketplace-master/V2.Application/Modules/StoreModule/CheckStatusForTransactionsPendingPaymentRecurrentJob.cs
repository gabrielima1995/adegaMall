using Domain.Services.StoreModule;
using V2.Application.Modules.StoreModule.Queries;

namespace V2.Application.Modules.StoreModule
{
    public class CheckStatusForTransactionsPendingPaymentRecurrentJob
    {
        private readonly IOrderQueries orderQueries;
        private readonly OrderService orderService;

        public CheckStatusForTransactionsPendingPaymentRecurrentJob(IOrderQueries orderQueries, OrderService orderService)
        {
            this.orderQueries = orderQueries;
            this.orderService = orderService;
        }

        public void Run()
        {
            var transactionIds = orderQueries.ListGatewayTransactionsPendingPayment();

            foreach (var id in transactionIds)
                orderService.ProcessGatewayPostback(id);
        }
    }
}