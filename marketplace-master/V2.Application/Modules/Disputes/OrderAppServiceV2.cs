using Domain.Entities.DisputeModule;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.StoreModule.Interfaces;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Text;
using V2.Application.Modules.Disputes;

namespace V2.Application.Modules.Products.Orders
{
    public class OrderAppServiceV2 : IOrderAppServiceV2
    {
        private const string roleAdmin = "Admin";
        private const string roleBuyer = "Buyer";
        private const string roleSystem = "System";

        private readonly IDisputeAppService disputeAppService;
        private readonly IDisputeRepository disputeRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderAppServiceV2(
            IDisputeAppService disputeAppService,
            IDisputeRepository disputeRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            this.disputeAppService = disputeAppService;
            this.disputeRepository = disputeRepository;
            this.orderRepository = orderRepository;
            this.unitOfWork = unitOfWork;
        }

        public OperationResult<Order> CloseDisputeAndCompleteOrder(Order order)
        {
            var hasDispute = order.Dispute_Id.HasValue;
            if (hasDispute)
            {
                var disputeResult = CloseDispute(order, roleSystem);
                if (disputeResult.IsFailure)
                    return disputeResult.Errors;
            }

            var additionalInfo = hasDispute ? GlobalizationHelper.GetString("ORDER_CLOSED_IDLE") : null;
            var statusResult = AdvanceOrderStatusToCompleted(order, additionalInfo);
            if (statusResult.IsFailure)
                return statusResult.Errors;

            var saveResult = unitOfWork.Save();
            if (saveResult.IsFailure)
                return saveResult.Errors;

            return order;
        }

        private OperationResult AdvanceOrderStatusToCompleted(Order order, string additionalInfo)
        {
            order.ChangeStatus(OrderStatus.Completed, null, additionalInfo);
            return orderRepository.Update(order);
        }

        private void ThrowCannotCloseDispute()
        {
            throw new InvalidOperationException(new StringBuilder()
                .AppendLine("04 out 2019")
                .AppendLine("Este método perdeu a função de fechar as disputas.")
                .AppendLine("Não conclui mais os pedidos com disputas em aberto.")
                .AppendLine()
                .AppendLine("O método V2.Application.Modules.Finance.TransactionWarrantyTransferJobs.AutoCloseOrders")
                .AppendLine("agora levanta somente os pedidos que não tenham disputa em aberto.")
                .AppendLine()
                .AppendLine("Todo pedido com disputa em aberto precisa ser manualmente fechado pelo admin ou comprador.")
                .ToString());
        }

        private OperationResult CloseDispute(Order order, string userRole)
        {
            ThrowCannotCloseDispute();

            var dispute = disputeRepository.GetById(order.Dispute_Id.Value);
            switch (userRole)
            {
                case roleAdmin:
                    dispute.SetStatus(DisputeStatus.ClosedByAdmin);
                    break;
                case roleBuyer:
                    dispute.SetStatus(DisputeStatus.ClosedByBuyer);
                    break;
                case roleSystem:
                    dispute.SetStatus(DisputeStatus.ClosedForInactivity);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid user role '{userRole}'.");
            }

            var updateResult = disputeRepository.Update(dispute);
            if (updateResult.IsFailure)
                return updateResult;

            var message = GlobalizationHelper.GetString("DISPUTE_SYSMESSAGE_PAYMENT_AUTHORIZED");
            var systemMessageResult = disputeAppService.AddSystemMessage(dispute, message);
            if (systemMessageResult.IsFailure)
                return systemMessageResult;

            return OperationResult.Success;
        }
    }
}