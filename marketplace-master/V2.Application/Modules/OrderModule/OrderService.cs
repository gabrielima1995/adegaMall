using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Events.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.StockManagement;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using V2.Application.Modules;

namespace Domain.Services.StoreModule
{
    public class OrderService
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly IBackgroundWorker backgroundWorker;
        private readonly IIocContainer container;
        private readonly IOrderRepository orderRepository;
        private readonly IPaymentService paymentService;
        private readonly IShippingService shippingService;
        private readonly CartService cartService;
        private readonly OrderTransactionService orderTransactionService;

        public OrderService(
            IBackgroundWorker backgroundWorker,
            IIocContainer container,
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            IShippingService shippingService,
            CartService cartService,
            OrderTransactionService orderTransactionService)
        {
            this.backgroundWorker = backgroundWorker;
            this.container = container;
            this.orderRepository = orderRepository;
            this.paymentService = paymentService;
            this.shippingService = shippingService;
            this.cartService = cartService;
            this.orderTransactionService = orderTransactionService;
        }

        public OperationResult<List<Order>> Create(string buyerId, string sessionId, PaymentOptions paymentOptions)
        {
            if (buyerId == null)
                throw new ArgumentNullException(nameof(buyerId));

            if (paymentOptions == null)
                throw new ArgumentNullException(nameof(paymentOptions));

            var cart = cartService.Get(buyerId);
            cart.SessionId = sessionId;

            if (config.Payment.GatewayIsPagarme())
                ConfigurePagarmePayment(cart);

            var result = CreateInternal(cart, paymentOptions);

            if (result.IsFailure)
                return result.Errors;

            return result;
        }

        private void ConfigurePagarmePayment(Cart cart)
        {
            var autoTransferEnabled = !config.Payment.Common.EnableTransactionWarranty && config.Payment.Common.RecipientTransferEnabled;

            foreach (var cartSeller in cart.Sellers)
                if (!cartSeller.Seller.GatewayTransferEnabled.HasValue || cartSeller.Seller.GatewayTransferEnabled.Value != autoTransferEnabled)
                    paymentService.SetAutoTransfer(cartSeller.Seller.Id, autoTransferEnabled);
        }

        private OperationResult<List<Order>> CreateInternal(Cart cart, PaymentOptions paymentOptions)
        {
            var creationWorkflow = container.Get<OrderService_CreateOrderWorkflow>();
            return creationWorkflow.CreateOrdersAndPay(cart, paymentOptions);
        }

        public Order GetById(Guid id)
        {
            return orderRepository.GetById(id);
        }

        public OrderDetails GetDetails(Guid orderId)
        {
            var order = orderRepository.GetDetails(orderId);

            if (order == null)
                return null;

            var shippingMethodAdapter = shippingService.GetAdapter(order.ShippingMethod);
            return new OrderDetails(order, shippingMethodAdapter);
        }

        public List<Order> ListByTransactionId(Guid transactionId)
        {
            return orderRepository.ListByTransactionId(transactionId);
        }

        public List<Order> ListForAdmin()
        {
            return orderRepository.ListForAdmin();
        }

        public OperationResult ReceiveGatewayPostback(TransactionPostback postback)
        {
            backgroundWorker.Enqueue<OrderService>(service => service.ReceiveGatewayPostbackInternal(postback.TransactionId));
            return OperationResult.Success;
        }

        public void ReceiveGatewayPostbackInternal(string transactionId)
        {
            var result = ProcessGatewayPostback(transactionId);

            if (result.IsFailure)
                throw new Exception("ReceiveGatewayPostbackInternal failed with error " + result.Errors.ToSimpleString());
        }

        public OperationResult ProcessGatewayPostback(string transactionId)
        {
            var getTransactionResult = paymentService.GetTransaction(transactionId);
            if (getTransactionResult.IsFailure)
                return getTransactionResult;

            var transaction = getTransactionResult.Result;
            var shouldHandleStatus = transaction.IsPaid() || transaction.IsPaymentCancelledOrRejected();
            if (!shouldHandleStatus)
                return OperationResult.Success;

            var domainTransaction = orderTransactionService.GetByGatewayId(transaction.TransactionId);
            var orders = orderRepository.ListByTransactionId(domainTransaction.Id);

            domainTransaction.Status = transaction.Status;
            foreach (var order in orders)
            {
                if (order.IsPaid)
                    continue;

                var orderDetails = orderRepository.GetDetails(order.Id);
                var shippingAdapter = shippingService.GetAdapter(order.ShippingMethod);

                if (transaction.IsPaymentCancelledOrRejected())
                    shippingAdapter.ChangeStatus(order, OrderStatus.Rejected, null, "Pagamento cancelado ou rejeitado na análise." + transaction.Message);
                else if (transaction.IsPaid())
                {
                    order.IsPaid = true;
                    shippingAdapter.ChangeStatusToOrderPaid(order);

                    if (MaaSConfig.Current.Payment.Common.EnableTransactionWarranty)
                        paymentService.ScheduleTransfersToSellersForWhenFundsAreAvailable(orderDetails);
                }

                orderRepository.Update(orderDetails);
            }

            var saveChangesResult = orderRepository.SaveChanges();
            return saveChangesResult;
        }

        public void SetOrderAutocompletionDate(Order order, DateTime serviceDeadline)
        {
            order.SetDeadlineAndAutoCompletionDates(serviceDeadline);
            orderRepository.Update(order);
        }

        public (bool canChange, bool showOption) CheckIfCanChangePeriodRental(OrderProduct orderProduct, IOrderProductRentalInfo rentalInfo)
        {
            var order = orderProduct.Order;

            var allowChangePeriodStatus = order.Status.NotIn(
                OrderStatus.AntifraudDenied,
                OrderStatus.DeliveryOnTheWay,
                OrderStatus.Delivered,
                OrderStatus.Canceled,
                OrderStatus.Rejected,
                OrderStatus.Returned,
                OrderStatus.Completed);

            if (order.IsRental && allowChangePeriodStatus)
            {
                if (orderProduct.ChangePeriodRentalPolicy == ChangePeriodRentalPolicy.Disallow)
                    return (false, true);

                if (orderProduct.ChangePeriodRentalPolicy == ChangePeriodRentalPolicy.Allow)
                    return (true, true);

                // data de início do agendamento
                var startDate = rentalInfo.StartDate.Value;
                var now = DateTime.UtcNow;
                var minHoursCanChange = orderProduct.ChangePeriodRentalPolicyHours;

                #region Calcula período de antecedência

                var timeDiff = startDate.Subtract(now);
                return (timeDiff.TotalHours > minHoursCanChange, true);

                #endregion Calcula período de antecedência
            }

            return (false, false);
        }

        public (bool canCancel, bool showOption) CheckIfCanCancelRental(Order order)
        {
            var allowCancelStatus = order.Status.NotIn(
                OrderStatus.AntifraudDenied,
                OrderStatus.DeliveryOnTheWay,
                OrderStatus.Delivered,
                OrderStatus.Canceled,
                OrderStatus.Rejected,
                OrderStatus.Returned,
                OrderStatus.Completed);

            if (order.IsRental && allowCancelStatus)
            {
                foreach (var orderProduct in order.Products)
                {
                    if (orderProduct.CancelOrderPolicy == CancelOrderPolicy.Disallow) continue;

                    var offsetHours = orderProduct.CancelOrderPolicy == CancelOrderPolicy.Allow
                        ? 0
                        : orderProduct.CancelOrderPolicyHours;

                    var untilUtc = DateTime.UtcNow
                        .Add(TimeSpan.FromHours(offsetHours));

                    var totalCompleted = orderProduct.GetTotalCompletedRentals(untilUtc);

                    if (totalCompleted < orderProduct.TotalPossibleRentals)
                        return (true, true);
                }

                return (false, true);
            }

            return (false, false);
        }
    }
}

namespace Domain.Modules.Products.Orders
{
    [Obsolete]
    public class OrderService_Obsolete
    {
        private readonly IRepository<OrderProduct> _orderProductRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderTransaction> _orderTransactionRepository;
        private readonly IPaymentService _paymentService;
        private readonly IShippingService _shippingService;
        private readonly IRepository<RentalTransaction> _rentalTransactionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly StockTransactionService _stockTransactionService;

        public OrderService_Obsolete(
            IRepository<OrderProduct> orderProductRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderTransaction> orderTransactionRepository,
            IPaymentService paymentService,
            IShippingService shippingService,
            IRepository<RentalTransaction> rentalTransactionRepository,
            IRepository<Product> productRepository,
            StockTransactionService stockTransactionService)
        {
            _orderProductRepository = orderProductRepository;
            _orderRepository = orderRepository;
            _orderTransactionRepository = orderTransactionRepository;
            _paymentService = paymentService;
            _shippingService = shippingService;
            _rentalTransactionRepository = rentalTransactionRepository;
            _productRepository = productRepository;
            _stockTransactionService = stockTransactionService;
        }

        public IList<Order> GetOrdersByBuyer(string buyerId)
        {
            return _orderRepository
                .Query(
                    i => i.Products,
                    i => i.Seller)
                .Where(i => i.Buyer.Id == buyerId)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();
        }

        public Order GetOrderDetailsFromBuyer(string buyerId, Guid orderId)
        {
            return _orderRepository
                .Get(i => i.Buyer.Id == buyerId && i.Id == orderId,
                    i => i.Seller,
                    i => i.Seller.Address,
                    i => i.ShippingAddress,
                    i => i.Products,
                    i => i.Transaction,
                    i => i.Histories,
                    i => i.Reviews);
        }

        public IList<Order> GetOrdersBySeller(string sellerId)
        {
            return _orderRepository
                .Query(
                    i => i.Products,
                    i => i.Buyer)
                .Where(i => i.Seller.Id == sellerId)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();
        }

        public Order GetOrderDetailsFromSeller(string sellerId, Guid orderId)
        {
            return _orderRepository
                .Get(i => i.Seller.Id == sellerId && i.Id == orderId,
                    i => i.Buyer,
                    i => i.ShippingAddress,
                    i => i.Products,
                    i => i.Transaction,
                    i => i.Histories,
                    i => i.Reviews);
        }

        public OperationResult NewStatusRental(Guid orderId, OrderStatus newStatus)
        {
            var order = _orderRepository.Get(i => i.Id == orderId,
                i => i.Products,
                i => i.Products.Select(j => j.Product));

            RentalTransactionType transactionStatus;

            switch (newStatus)
            {
                case OrderStatus.Delivered:
                    order.Delivered();
                    transactionStatus = RentalTransactionType.Delivery;
                    break;

                case OrderStatus.Returned:
                case OrderStatus.Rejected:
                    order.Returned();
                    transactionStatus = RentalTransactionType.Returned;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus));
            }

            foreach (var item in order.Products)
            {
                var transaction = RentalTransaction.Create(item, item.Product, transactionStatus);
                _rentalTransactionRepository.Insert(transaction);
                _orderProductRepository.Update(item);
                _productRepository.Update(item.Product);
            }

            _orderRepository.Update(order);
            _orderRepository.SaveChanges();

            return OperationResult.Success;
        }

        public decimal CalculateTotalRefund(Order order)
        {
            if (!order.IsRental)
                return order.Amount;

            var totalRefund = 0M;

            foreach (var orderProduct in order.Products)
            {
                if (orderProduct.CancelOrderPolicy == CancelOrderPolicy.Disallow) continue;

                #region Reembolsar agendamentos não completados

                var offsetHours = orderProduct.CancelOrderPolicy == CancelOrderPolicy.Allow
                    ? 0
                    : orderProduct.CancelOrderPolicyHours;

                var untilUtc = DateTime.UtcNow
                    .Add(TimeSpan.FromHours(offsetHours));

                var totalCompletedRentals = orderProduct.GetTotalCompletedRentals(untilUtc);
                var amountRental = orderProduct.IsPackageProduct
                    ? orderProduct.GetCreditPrice()
                    : orderProduct.SubTotal;
                var amountCompletedRentals = Math.Round(totalCompletedRentals * amountRental, 2);

                totalRefund += orderProduct.SubTotal - amountCompletedRentals;

                #endregion Reembolsar agendamentos não completados
            }

            return totalRefund;
        }

        public OperationResult CancelOrReject(Guid orderId)
        {
            var order = _orderRepository.Get(orderId);

            #region Estornar pagamento

            if (order.IsPaid)
            {
                if (order.Transaction_Id != null)
                {
                    var transaction = _orderTransactionRepository.Get(order.Transaction_Id);
                    var totalRefund = CalculateTotalRefund(order);
                    var refundResult = _paymentService.Refund(order, transaction, totalRefund);
                    if (!refundResult.OK) return refundResult.Errors;
                }

                order.IsPaid = false;
            }

            #endregion Estornar pagamento

            #region Retornar produto ao estoque

            if (order.IsSale)
            {
                var orderProducts = _orderProductRepository
                    .Query(x => x.Product, x => x.Product.StockVariants, x => x.Variant)
                    .Where(x => x.Order.Id == order.Id)
                    .ToList();

                foreach (var item in orderProducts)
                {
                    var stockTran = new StockTransaction(order, item.Product, item.Variant, item.Quantity, StockTransactionType.SaleCancelledOrRejected);
                    _stockTransactionService.Add(stockTran);
                }

                _stockTransactionService.CommitPending();
            }

            #endregion Retornar produto ao estoque

            _orderTransactionRepository.SaveChanges();
            return OperationResult.Success;
        }

        public OperationResult ChangeStatusBySeller(string sellerId, Guid orderId, OrderStatus newStatus, string additionalInfo)
        {
            var order = _orderRepository.Get(
                o => o.Seller.Id == sellerId && o.Id == orderId,
                o => o.Seller,
                o => o.Buyer,
                o => o.ShippingAddress,
                o => o.Products);

            if (order == null)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_NOT_FOUND_MESSAGE"));

            return ChangeStatus(order, newStatus, additionalInfo, order.Seller);
        }

        public OperationResult ChangeStatusByBuyer(string buyerId, Guid orderId, OrderStatus newStatus, string additionalInfo)
        {
            var order = _orderRepository.Get(
                o => o.Buyer.Id == buyerId && o.Id == orderId,
                o => o.Seller,
                o => o.Buyer,
                o => o.ShippingAddress,
                o => o.Products);

            if (order == null)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_NOT_FOUND_MESSAGE"));

            return ChangeStatus(order, newStatus, additionalInfo, order.Buyer);
        }

        private OperationResult ChangeStatus(Order order, OrderStatus newStatus, string additionalInfo, Person person)
        {
            var shippginMethodAdapter = _shippingService.GetAdapter(order.ShippingMethod);
            var changeStatusResult = shippginMethodAdapter.ChangeStatus(order, newStatus, person, additionalInfo);
            if (!changeStatusResult.OK) return changeStatusResult.Errors;

            if (newStatus == OrderStatus.Canceled && order.RentalInfo != null)
                order.AddDomainEvent(new RentalCancelled(order));

            _orderRepository.Update(order);
            _orderRepository.SaveChanges();

            return OperationResult.Success;
        }

        public OperationResult Review(Guid orderId, Person reviewer, int rate, string description)
        {
            var order = _orderRepository.Get(o => o.Id == orderId,
                o => o.Buyer,
                o => o.Seller,
                o => o.Reviews);

            if (order == null)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_NOT_FOUND_MESSAGE"));

            var shippingMethodAdapter = _shippingService.GetAdapter(order.ShippingMethod);
            var validation = order.Review(reviewer, rate, description, shippingMethodAdapter);

            if (!validation.OK)
                return validation;

            _orderRepository.Update(order);
            _orderRepository.SaveChanges();

            return new OperationResult();
        }
    }
}