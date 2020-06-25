using Application.Config;
using Domain.Entities.StoreModule;
using Domain.Modules.Products.Orders;
using Framework;
using System;
using System.Linq;
using Postbacks = PaymentIntegrator.PagarMe.Postbacks;

namespace Application.Modules.Subscriptions.PagarMe
{
    public class PagarMePostbackGateway
    {
        private const string PaidStatus = "paid";

        private readonly OrderService_Obsolete _orderService;
        private readonly IRepository<Order> _orderRepository;

        public string NotifyUrl => MaaSConfig.Current.Environment.AppUrl + "pagarme/boletopostback/";

        public PagarMePostbackGateway(
            OrderService_Obsolete orderService,
            IRepository<Order> orderRepository)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
        }

        public OperationResult ProcessPostback(Postbacks.Postback postback)
        {
            var orders = _orderRepository
                .Query(x => x.Transaction)
                .Where(x => x.Transaction.TransactionId == postback.model_id)
                .ToList();

            var isPaid = PaidStatus.Equals(postback.status, StringComparison.OrdinalIgnoreCase);

            if (isPaid)
            {
                foreach (var order in orders)
                {
                    if (order.Status == OrderStatus.PendingPayment)
                    {
                        // TODO: fazer mudança de status do pedido.
                    }
                }
            }

            return new OperationResult();
        }
    }
}