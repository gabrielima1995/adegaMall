using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.StockManagement;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Domain.Modules.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Domain.Services.StoreModule
{
    internal class OrderService_CreateOrderWorkflow
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly CreateAndSaveOrders createAndSaveOrders;

        private readonly CartService cartService;
        private readonly CreditCardService creditCardService;
        private readonly OrderService orderService;
        private readonly IPaymentService paymentService;
        private readonly IShippingService shippingService;
        private readonly StockTransactionService stockTransactionService;
        private readonly RentalService _rentalService;

        private readonly ICartRepository cartRepository;
        private readonly IOrderTransactionRepository orderTransactionRepository;
        private readonly IQuoteBillingRepository quoteBillingRepository;
        private readonly IQuoteRepository quoteRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderService_CreateOrderWorkflow(
            CreateAndSaveOrders createAndSaveOrders,

            CartService cartService,
            CreditCardService creditCardService,
            OrderService orderService,
            IPaymentService paymentService,
            IShippingService shippingService,
            StockTransactionService stockTransactionService,
            RentalService rentalService,

            ICartRepository cartRepository,
            IOrderTransactionRepository orderTransactionRepository,
            IQuoteBillingRepository quoteBillingRepository,
            IQuoteRepository quoteRepository,
            IUnitOfWork unitOfWork)
        {
            this.createAndSaveOrders = createAndSaveOrders;

            this.cartService = cartService;
            this.creditCardService = creditCardService;
            this.orderService = orderService;
            this.paymentService = paymentService;
            this.shippingService = shippingService;
            this.stockTransactionService = stockTransactionService;
            _rentalService = rentalService;

            this.cartRepository = cartRepository;
            this.orderTransactionRepository = orderTransactionRepository;
            this.quoteBillingRepository = quoteBillingRepository;
            this.quoteRepository = quoteRepository;
            this.unitOfWork = unitOfWork;
        }

        public OperationResult<List<Order>> CreateOrdersAndPay(Cart cart, PaymentOptions paymentOptions)
        {
            var createAndPayResults = CreateOrdersAndPayInternal(cart, paymentOptions);
            if (createAndPayResults.IsFailure)
                return createAndPayResults.Errors;

            var orders = createAndPayResults.Result;

            var sideEffectsResults = PropagateSideEffects(cart, orders);
            if (sideEffectsResults.IsFailure)
                return sideEffectsResults.Errors;

            return new OperationResult<List<Order>>(orders);
        }

        public OperationResult<List<Order>> CreateOrdersAndPayInternal(Cart cart, PaymentOptions paymentOptions)
        {
            var preparationResult = CheckAndPrepare(cart, paymentOptions);
            if (preparationResult.IsFailure)
                return preparationResult.Errors;

            var createOrdersResult = createAndSaveOrders.Execute(cart, paymentOptions);
            if (createOrdersResult.IsFailure)
                return createOrdersResult.Errors;

            var orders = createOrdersResult.Result;

            if (config.Quote.Enabled && config.Quote.QuoteBilling.Enabled)
            {
                var billingResult = SetBillingOrderIdFromNewOrdersCartId(cart.Id, orders);
                if (billingResult.IsFailure)
                    return billingResult.Errors;
            }

            if (paymentOptions.HasPayment())
            {
                var processPaymentResult = ProcessPayment(orders, cart.Buyer, paymentOptions);
                if (processPaymentResult.IsFailure)
                    return processPaymentResult.Errors;
            }

            cart.Completed = true;
            cartRepository.Update(cart);

            var saveResult = unitOfWork.Save();
            if (saveResult.IsFailure)
                return saveResult.Errors;

            return new OperationResult<List<Order>>(orders);
        }

        private OperationResult CheckAndPrepare(Cart cart, PaymentOptions paymentOptions)
        {
            var preparationErrors = new ErrorDictionary();

            CheckAndPreparePayment(cart, paymentOptions)
                .OnSuccess(() => ValidateCart(cart))
                .OnFailure((errors) => preparationErrors = errors);

            return preparationErrors;
        }

        private OperationResult CheckAndPreparePayment(Cart cart, PaymentOptions paymentOptions)
        {
            switch (paymentOptions.Method)
            {
                case PaymentMethod.CreditCard:
                    {
                        if (!config.Payment.Enabled || !config.Payment.PaymentGateway.CreditCard.Enabled)
                            return new OperationResult(nameof(paymentOptions.Method), $"Payment method {paymentOptions.Method} is not enabled.");

                        paymentOptions.Installments = Math.Max(1, Math.Min(paymentOptions.Installments, config.Payment.PaymentGateway.Installments.MaxInstallments));

                        var creditCardResult = creditCardService.CreateOnGateway(cart.Buyer, paymentOptions.CreditCard);
                        if (!creditCardResult.OK) return creditCardResult.Errors;

                        paymentOptions.CreditCardResult = creditCardResult.Result;
                        break;
                    }

                case PaymentMethod.Boleto:
                    {
                        if (!config.Payment.Enabled || !config.Payment.PaymentGateway.Boleto.Enabled)
                            return new OperationResult(nameof(paymentOptions.Method), $"Payment method {paymentOptions.Method} is not enabled.");

                        paymentOptions.Installments = 1;
                        break;
                    }

                case PaymentMethod.Offline:
                    {
                        if (!config.Product.Enabled || !config.Product.Order.OfflinePaymentEnabled)
                            return new OperationResult(nameof(paymentOptions.Method), $"Payment method {paymentOptions.Method} is not enabled.");

                        paymentOptions.Installments = 1;
                        break;
                    }

                default:
                    return new OperationResult(nameof(paymentOptions.Method), $"Payment method #{paymentOptions.Method} is not handled.");
            }

            return OperationResult.Success;
        }

        private OperationResult SetBillingOrderIdFromNewOrdersCartId(Guid cartId, IEnumerable<Order> orders)
        {
            var relatedIds = quoteBillingRepository.ListBillingVsOrderIdsFromCartId(cartId);

            if (relatedIds.Any())
            {
                foreach (var pair in relatedIds)
                {
                    var billing = quoteBillingRepository.Get(pair.QuoteBillingId);
                    billing.SetOrderId(pair.OrderId);
                    quoteBillingRepository.Update(billing);

                    var order = orders.Single(o => o.Id == pair.OrderId);
                    var quote = quoteRepository.GetById(billing.QuoteId);

                    if (MaaSConfig.Current.Payment.Common.EnableTransactionWarranty)
                        orderService.SetOrderAutocompletionDate(order, quote.Deadline.Value);
                }

                return unitOfWork.Save();
            }

            return OperationResult.Success;
        }

        private OperationResult<OrderTransaction> ProcessPayment(List<Order> orders, Buyer buyer, PaymentOptions paymentOptions)
        {
            if (paymentOptions.Method == PaymentMethod.Offline)
                throw new InvalidOperationException($"Payment method \"{PaymentMethod.Offline}\" cannot be processed.");

            var paymentResult = paymentService.Pay(orders, buyer, paymentOptions);
            if (paymentResult.IsFailure) return paymentResult.Errors;

            var transaction = paymentResult.Result;
            var orderTransaction = new OrderTransaction(transaction, orders);

            var orderTransactionResult = orderTransactionRepository.Add(orderTransaction);
            if (orderTransactionResult.IsFailure) return orderTransactionResult.Errors;

            foreach (var order in orders)
                if (transaction.IsPaid())
                    order.IsPaid = true;

            return orderTransaction;
        }

        private OperationResult PropagateSideEffects(Cart cart, List<Order> orders)
        {
            var shippingMethodAdapter = shippingService.GetAdapter(cart.ShippingMethod);

            foreach (var order in orders)
            {
                shippingMethodAdapter.ChangeStatusForNewOrder(order);

                if (config.Payment.Common.EnableTransactionWarranty && order.PaymentMethod == PaymentMethod.CreditCard /*quando o pgto é por boleto, isso é feito no webhook do pgto identificado*/)
                    paymentService.ScheduleTransfersToSellersForWhenFundsAreAvailable(order);
            }

            var finishingErrors = new ErrorDictionary();

            RegisterStockTransactions(cart, orders)
                .OnSuccess(() => RegisterRecurringRental(orders))
                .OnSuccess(() => unitOfWork.Save())
                .OnFailure(errors => finishingErrors.AddRange(errors));

            return finishingErrors;
        }

        private OperationResult RegisterStockTransactions(Cart cart, List<Order> orders)
        {
            if (config.Product.BusinessModel.Sale.Enabled && !config.Product.BusinessModel.Rental.Enabled)
            {
                foreach (var item in cart.Items)
                {
                    var order = orders.First(i => i.Seller.Id == item.Product.Store.SellerId);
                    var stockTran = new StockTransaction(order, item.Product, item.Variant, item.Quantity * -1M, StockTransactionType.NewSale);
                    stockTransactionService.Add(stockTran);
                }

                stockTransactionService.CommitPending();
            }

            return OperationResult.Success;
        }

        private OperationResult RegisterRecurringRental(List<Order> orders)
        {
            if (config.Product.BusinessModel.Rental.Enabled &&
                config.Product.BusinessModel.Rental.ProductAvailability.Enabled &&
                config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled)
            {
                foreach (var order in orders)
                {
                    foreach (var orderProduct in order.Products
                        .Where(i => i.IsPackageProduct && i.RentalInfo.ProductAvailability_Id.HasValue))
                    {
                        var operation = _rentalService.RegisterRecurringRental(orderProduct, false);

                        if (operation.IsFailure) return operation;
                    }
                }
            }

            return OperationResult.Success;
        }

        private OperationResult ValidateCart(Cart cart)
        {
            if (cart.StockAndShippingValidation.IsFailure)
                return cart.StockAndShippingValidation.Errors;

            if (cart.Amount < config.Product.Order.MinOrderTotal)
            {
                var msg = string.Format(CultureInfo.CurrentCulture.GetStringLocalized("ORDER_MIN_AMOUNT_NOT_MET"),
                    config.DefaultCurrency,
                    config.Product.Order.MinOrderTotal);

                return new ErrorDictionary("", msg);
            }

            if (config.Product.Order.MinOrderBySeller > 0M)
            {
                var itemsBySeller = cart.Items.GroupBy(x => x.Product.Store.SellerId);
                var minShippingErrors = new ErrorDictionary();

                foreach (var group in itemsBySeller)
                {
                    var sum = group.Sum(x => x.SubTotal);

                    if (sum < config.Product.Order.MinOrderBySeller)
                    {
                        var msg = string.Format(CultureInfo.CurrentCulture.GetStringLocalized("ORDER_MIN_BY_SELLER_NOT_MET"),
                            group.First().Product.Store.Seller.PublicProfileName,
                            config.DefaultCurrency,
                            config.Product.Order.MinOrderTotal,
                            config.Product.Order.MinOrderTotal - sum);

                        minShippingErrors.Add("", msg);
                    }
                }

                if (minShippingErrors.HasErrors)
                    return minShippingErrors;
            }

            return OperationResult.Success;
        }
    }

    public class CreateAndSaveOrders
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly RentalService rentalService;
        private readonly IOrderRepository orderRepository;

        public CreateAndSaveOrders(RentalService rentalService, IOrderRepository orderRepository)
        {
            this.rentalService = rentalService;
            this.orderRepository = orderRepository;
        }

        public OperationResult<List<Order>> Execute(Cart cart, PaymentOptions paymentOptions)
        {
            var orders = CreateOrdersBySeller(cart, paymentOptions);

            var errors = ErrorDictionary.FromMany(orders.Select(order => order.Errors));
            if (errors.HasErrors)
                return errors;

            foreach (var order in orders) orderRepository.Add(order);
            var saveOrdersResult = orderRepository.SaveChanges();
            if (!saveOrdersResult.OK) return saveOrdersResult.Errors;

            return orders;
        }

        private List<Order> CreateOrdersBySeller(Cart cart, PaymentOptions paymentOptions)
        {
            var ordersBySeller = new List<Order>(cart.Sellers.Count);

            foreach (var cartSeller in cart.Sellers)
            {
                var sellerItems = GetSellerItems(cart, cartSeller.Seller.Id);
                var ordersByDelivery = CreateOrdersBySellerByDelivery(cart, paymentOptions, cartSeller, sellerItems);
                ordersBySeller.AddRange(ordersByDelivery);
            }

            return ordersBySeller;
        }

        private List<Order> CreateOrdersBySellerByDelivery(Cart cart, PaymentOptions paymentOptions, CartSeller cartSeller, List<CartItem> sellerItems)
        {
            var deliveries = ConfigureDeliveries(sellerItems);
            var orders = new List<Order>(deliveries.Count);

            foreach (var delivery in deliveries)
            {
                var order = CreateOrderWithProducts(cart, cartSeller, paymentOptions, deliveries, delivery);

                if (config.Payment.Common.EnableTransactionWarranty && config.Product.BusinessModel.Rental.Enabled)
                {
                    // com pelo menos um agendamento
                    if (order.Products.Any(i => i.MaxRentalEndDate.HasValue))
                    {
                        // maior data de um agendamento
                        var deadline = order.Products.Where(i => i.MaxRentalEndDate.HasValue).Max(p => p.MaxRentalEndDate.Value);

                        order.SetDeadlineAndAutoCompletionDates(deadline);
                    }
                }

                if (config.Payment.GatewayIsWirecard() && config.Payment.Moip.Options.CustomerPaysFees)
                    order.InstallmentFees = CalculateOrderInstallmentFeesForWirecard(cart, paymentOptions);

                orders.Add(order);
            }

            return orders;
        }

        private Order CreateOrderWithProducts(Cart cart, CartSeller cartSeller, PaymentOptions paymentOptions, List<List<CartItem>> deliveryList, List<CartItem> deliveryItems)
        {
            var isRental =
                config.Product.BusinessModel.Rental.Enabled &&
                deliveryItems.Any(item => item.RentalInfo != null && item.RentalInfo.HasValue);

            var orderShipping = CalculateOrderShipping(cartSeller, deliveryList, deliveryItems);

            var order = new Order(
                cart.Id,
                paymentOptions.Method,
                cart.Buyer,
                cartSeller.Seller,
                cart.ShippingAddress,
                cart.ShippingMethod,
                orderShipping,
                GetBusinessModel(), isRental)
            {
                ExtraInfo = cart.ExtraInfo
            };

            foreach (var item in deliveryItems)
                order.AddProduct(item, rentalService);

            var marketplaceFee = CalculateOrderMarketplaceFee(order);
            order.SetStartupFee(marketplaceFee);

            return order;
        }

        private List<List<CartItem>> ConfigureDeliveries(List<CartItem> sellerItems)
        {
            if (config.Product.BusinessModel.Rental.Enabled)
                return sellerItems
                    .GroupBy(i => i.RentalInfo.Hash, i => i, (key, values) => values.ToList())
                    .ToList();

            return new List<List<CartItem>> { sellerItems };
        }

        private decimal CalculateOrderMarketplaceFee(Order order)
        {
            var marketplaceFee = order.Seller.GetCurrentPaymentAmountFee();

            if (order.Seller.GetCurrentPaymentPercentFee() > 0M)
                marketplaceFee += order.Amount * order.Seller.GetCurrentPaymentPercentFee() / 100M;

            return marketplaceFee;
        }

        private decimal CalculateOrderShipping(CartSeller cartSeller, List<List<CartItem>> deliveryList, List<CartItem> deliveryItem)
        {
            return (cartSeller.SellerShipping / deliveryList.Count) + deliveryItem.Sum(x => x.Shipping);
        }

        private decimal CalculateOrderInstallmentFeesForWirecard(Cart cart, PaymentOptions paymentOptions)
        {
            var anticipationFee = config.Payment.Moip.Options.FeesConfig.AnticipationFees[paymentOptions.Installments] / 100m;
            var transactionFee = config.Payment.Moip.Options.FeesConfig.BaseFee / 100m;

            var installmentFees = cart.Amount * ((1 - transactionFee) / (1 - transactionFee - anticipationFee)) - cart.Amount;
            installmentFees = (installmentFees / paymentOptions.Installments).RoundTo2Places();
            installmentFees *= paymentOptions.Installments;

            return installmentFees;
        }

        private ProductBusinessModel GetBusinessModel()
        {
            return config.Product.BusinessModel.Sale.Enabled ? ProductBusinessModel.Sale : ProductBusinessModel.Rental;
        }

        private List<CartItem> GetSellerItems(Cart cart, string sellerId)
        {
            return cart.Items
                .Where(x => x.Product.Store.SellerId == sellerId)
                .ToList();
        }
    }
}