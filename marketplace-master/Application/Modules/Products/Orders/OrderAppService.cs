using Application.Config;
using Domain;
using Domain.Entities;
using Domain.Entities.DisputeModule;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.Orders;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;
using V2.Application.Messages;
using V2.Application.Modules.Disputes;
using V2.Application.Modules.StoreRequest.Messages;
using V2.Application.Modules.StoreRequest.Messages.Specs;

namespace Application.Modules.Products.Orders
{
    public class OrderAppService
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        private readonly OrderService_Obsolete orderService;
        private readonly OrderService _orderServiceV2;
        private readonly PersonService personService;
        private readonly IDisputeRepository disputeRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IDisputeAppService disputeAppService;
        private readonly RentalService _rentalService;
        private readonly IRepository<OrderProduct> _orderProductRepository;

        public OrderAppService(
            OrderService_Obsolete orderService,
            OrderService orderServiceV2,
            PersonService personService,
            IDisputeRepository disputeRepository,
            IOrderRepository orderRepository,
            IDisputeAppService disputeAppService,
            RentalService rentalService,
            IRepository<OrderProduct> orderProductRepository)
        {
            this.orderService = orderService;
            _orderServiceV2 = orderServiceV2;
            this.personService = personService;
            this.disputeRepository = disputeRepository;
            this.orderRepository = orderRepository;
            this.disputeAppService = disputeAppService;
            _rentalService = rentalService;
            _orderProductRepository = orderProductRepository;
        }

        public MyOrdersViewModel GetMyOrders(string buyerId)
        {
            var orders = orderService.GetOrdersByBuyer(buyerId);
            return new MyOrdersViewModel(orders);
        }

        public MySalesViewModel GetMySales(string sellerId, bool showCanceled)
        {
            var orders = orderService.GetOrdersBySeller(sellerId, showCanceled);
            return new MySalesViewModel(orders);
        }

        public OperationResult ChangeStatusBySeller(string sellerId, Guid orderId, OrderStatus status, string additionalInfo = null)
        {
            return orderService.ChangeStatusBySeller(sellerId, orderId, status, additionalInfo);
        }

        public OperationResult ChangeStatusByBuyer(string buyerId, Guid orderId, OrderStatus status, string additionalInfo = null)
        {
            if (status == OrderStatus.Canceled)
            {
                var order = orderRepository.Get(
                    x => x.Buyer.Id == buyerId && x.Id == orderId,
                    x => x.Products,
                    x => x.Products.Select(j => j.RentalsInfo));

                var (canCancel, _) = _orderServiceV2.CheckIfCanCancelRental(order);

                if (!canCancel)
                    return OperationResult.Fail("Can't cancel rental.");
            }

            return orderService.ChangeStatusByBuyer(buyerId, orderId, status, additionalInfo);
        }

        public OperationResult Review(OrderReviewViewModel model)
        {
            var reviewer = personService.Get(i => i.Id == model.ReviewerId);
            return orderService.Review(model.OrderId, reviewer, model.Rate, model.Description);
        }

        public OperationResult<Order> AuthorizePayment(Order order, Person actingPerson, string userRole)
        {
            order.ChangeStatus(OrderStatus.Completed, actingPerson);

            var result = orderRepository.Update(order, true);
            if (result.IsFailure)
                return result.Errors;

            if (order.Dispute_Id.HasValue)
            {
                var dispute = disputeRepository.GetById(order.Dispute_Id.Value);
                switch (userRole)
                {
                    case "Admin":
                        dispute.SetStatus(DisputeStatus.ClosedByAdmin);
                        break;

                    case "Buyer":
                        dispute.SetStatus(DisputeStatus.ClosedByBuyer);
                        break;

                    case "System":
                        dispute.SetStatus(DisputeStatus.ClosedForInactivity);
                        break;

                    default:
                        return OperationResult<Order>.Fail("error", "Invalid user role");
                }

                disputeRepository.Update(dispute, true);

                var message = GlobalizationHelper.GetString("DISPUTE_SYSMESSAGE_PAYMENT_AUTHORIZED");
                disputeAppService.AddSystemMessage(dispute, message);
            }

            return order;
        }

        private DateTime CalculateRentalEndDateTime(DateTime rentalStartDateTime, OrderProductRentalInfo rentalInfo)
        {
            switch (rentalInfo.RentalPeriodUnit)
            {
                case RentalPeriodUnit.Minute:
                    return rentalStartDateTime.AddMinutes((double)rentalInfo.RentalPeriodValue);

                case RentalPeriodUnit.Hour:
                    return rentalStartDateTime.AddHours((double)rentalInfo.RentalPeriodValue);

                case RentalPeriodUnit.Day:
                    return rentalStartDateTime.AddDays((double)rentalInfo.RentalPeriodValue);

                case RentalPeriodUnit.Month:
                    return rentalStartDateTime.AddMonths((int)rentalInfo.RentalPeriodValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(rentalInfo.RentalPeriodUnit));
            }
        }

        public OperationResult ChangeRentalProductPeriodByBuyer(string buyerId, ChangeRentalProductPeriodViewModel model, MarketplaceTimeZone timeZone)
        {
            if (!config.Product.BusinessModel.Rental.Enabled ||
                !config.Product.BusinessModel.Rental.Buyer.ChangePeriod.Enabled) return OperationResult.Fail("Feature unallowed.");

            var orderProduct = _orderProductRepository
                .Query(x => x.Product, x => x.Order, x => x.Order.Seller, x => x.Order.Buyer, x => x.RentalsInfo)
                .FirstOrDefault(x => x.Id == model.OrderProductId && x.Order.Buyer.Id == buyerId);

            if (orderProduct == null) return OperationResult.Fail("Order not found.");
            if (!orderProduct.Order.IsRental) return OperationResult.Fail("Order is not rental.");

            #region Verifica se pode alterar o agendamento

            var getRentalInfo = orderProduct.GetRentalInfo(model.OrderProductRentalInfoId);

            if (getRentalInfo.IsFailure) return getRentalInfo;

            var rentalInfo = getRentalInfo.Result;

            var (canChangePeriod, _) = _orderServiceV2.CheckIfCanChangePeriodRental(orderProduct, rentalInfo);

            if (!canChangePeriod)
                return OperationResult.Fail("Can't change period rental.");

            #endregion Verifica se pode alterar o agendamento

            // Calcula a data final da locação
            model.RentalEndDateTime = CalculateRentalEndDateTime(model.RentalStartDateTime.Value, orderProduct.RentalInfo);

            var rentalStartUtc = timeZone.ConvertTimeToUtc(model.RentalStartDateTime.Value);
            var rentalEndUtc = timeZone.ConvertTimeToUtc(model.RentalEndDateTime.Value);

            #region Verifica a disponibilidade

            var isAvaliable = _rentalService.CheckAvailability(orderProduct.Product, model.Quantity, rentalStartUtc, rentalEndUtc, timeZone, model.OrderProductId, model.OrderProductRentalInfoId,
                model.ProductAvailabilityId);

            if (isAvaliable.IsFailure)
                return isAvaliable;

            #endregion Verifica a disponibilidade

            #region Altera o periodo de locação

            var changeResult = orderProduct.ChangeRental(rentalStartUtc, rentalEndUtc, model.OrderProductRentalInfoId, model.ProductAvailabilityId);

            if (changeResult.IsFailure) return changeResult;

            _orderProductRepository.Update(orderProduct);
            orderRepository.SaveChanges();

            #endregion Altera o periodo de locação

            #region Email para vendedor

            var timeZoneSeller = orderProduct.Order.Seller.PreferredTimeZone;
            var cultureSeller = orderProduct.Order.Seller.PreferredCulture;
            MessagesContext.Current
                .GenerateAndSend<RentalPeriodChangedMessageToSellerSpecification, RentalPeriodChangedMessageModel>(
                    new RentalPeriodChangedMessageModel
                    {
                        BuyerEmail = orderProduct.Order.Buyer.Email,
                        BuyerName = orderProduct.Order.Buyer.Name,
                        OrderId = orderProduct.Order.Id.DisplayShortGuid(),
                        SellerName = orderProduct.Order.Seller.AlternativeName ?? orderProduct.Order.Seller.Name,
                        ProductName = orderProduct.Product.Title,
                        ExtraInfo = orderProduct.Order.ExtraInfo,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStartUtc, timeZoneSeller, cultureSeller),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEndUtc, timeZoneSeller, cultureSeller)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { orderProduct.Order.Seller.Email },
                    orderProduct.Order.Seller.Lang);

            #endregion Email para vendedor

            #region Email para comprador

            var timeZoneBuyer = orderProduct.Order.Buyer.PreferredTimeZone;
            var cultureBuyer = orderProduct.Order.Buyer.PreferredCulture;
            MessagesContext.Current
                .GenerateAndSend<RentalPeriodChangedMessageToBuyerSpecification, RentalPeriodChangedMessageModel>(
                    new RentalPeriodChangedMessageModel
                    {
                        BuyerEmail = orderProduct.Order.Buyer.Email,
                        BuyerName = orderProduct.Order.Buyer.Name,
                        OrderId = orderProduct.Order.Id.DisplayShortGuid(),
                        SellerName = orderProduct.Order.Seller.AlternativeName ?? orderProduct.Order.Seller.Name,
                        ProductName = orderProduct.Product.Title,
                        ExtraInfo = orderProduct.Order.ExtraInfo,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStartUtc, timeZoneBuyer, cultureBuyer),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEndUtc, timeZoneBuyer, cultureBuyer)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { orderProduct.Order.Buyer.Email },
                    orderProduct.Order.Buyer.Lang); ;

            #endregion Email para comprador

            return OperationResult.Success;
        }

        public OperationResult NewRentalProductPeriodByBuyer(string buyerId, NewRentalProductPeriodViewModel model, MarketplaceTimeZone timeZone)
        {
            if (!config.Product.BusinessModel.Rental.Enabled) return OperationResult.Fail("Rental unallowed.");

            var orderProduct = _orderProductRepository
                .Query(x => x.Product, x => x.Order, x => x.Order.Seller, x => x.Order.Buyer)
                .FirstOrDefault(x => x.Id == model.OrderProductId && x.Order.Buyer.Id == buyerId);

            if (orderProduct == null) return OperationResult.Fail("Order product not found.");
            if (!orderProduct.Order.IsRental) return OperationResult.Fail("Order is not rental.");
            if (orderProduct.Order.Status == OrderStatus.Canceled) return OperationResult.Fail("Order is canceled.");
            if (!orderProduct.IsPackageProduct) return OperationResult.Fail("Order product is not package.");

            #region Verifica se pode realizar novo agendamento

            if (orderProduct.RentalsInfo.Count >= orderProduct.PackageCredits.GetValueOrDefault()) return OperationResult.Fail("Package credits unavailable.");

            #endregion Verifica se pode realizar novo agendamento

            // Calcula a data final da locação
            model.RentalEndDateTime = CalculateRentalEndDateTime(model.RentalStartDateTime.Value, orderProduct.RentalInfo);

            var rentalStartUtc = timeZone.ConvertTimeToUtc(model.RentalStartDateTime.Value);
            var rentalEndUtc = timeZone.ConvertTimeToUtc(model.RentalEndDateTime.Value);

            #region Verifica a disponibilidade

            var isAvaliable = _rentalService.CheckAvailability(orderProduct.Product, model.Quantity, rentalStartUtc, rentalEndUtc, timeZone,
                productAvailabilityId: model.ProductAvailabilityId);

            if (isAvaliable.IsFailure)
                return isAvaliable;

            #endregion Verifica a disponibilidade

            #region Cria um novo período de locação

            var newResult = orderProduct.NewRental(rentalStartUtc, rentalEndUtc, model.ProductAvailabilityId);

            if (newResult.IsFailure) return newResult;

            _orderProductRepository.Update(orderProduct);
            orderRepository.SaveChanges();

            #endregion Cria um novo período de locação

            #region Email para vendedor

            var timeZoneSeller = orderProduct.Order.Seller.PreferredTimeZone;
            var cultureSeller = orderProduct.Order.Seller.PreferredCulture;
            MessagesContext.Current
                .GenerateAndSend<RentalNewPeriodMessageToSellerSpecification, RentalPeriodChangedMessageModel>(
                    new RentalPeriodChangedMessageModel
                    {
                        BuyerEmail = orderProduct.Order.Buyer.Email,
                        BuyerName = orderProduct.Order.Buyer.Name,
                        OrderId = orderProduct.Order.Id.DisplayShortGuid(),
                        SellerName = orderProduct.Order.Seller.AlternativeName ?? orderProduct.Order.Seller.Name,
                        ProductName = orderProduct.Product.Title,
                        ExtraInfo = orderProduct.Order.ExtraInfo,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStartUtc, timeZoneSeller, cultureSeller),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEndUtc, timeZoneSeller, cultureSeller)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { orderProduct.Order.Seller.Email },
                    orderProduct.Order.Seller.Lang);

            #endregion Email para vendedor

            #region Email para comprador

            var timeZoneBuyer = orderProduct.Order.Buyer.PreferredTimeZone;
            var cultureBuyer = orderProduct.Order.Buyer.PreferredCulture;
            MessagesContext.Current
                .GenerateAndSend<RentalNewPeriodMessageToBuyerSpecification, RentalPeriodChangedMessageModel>(
                    new RentalPeriodChangedMessageModel
                    {
                        BuyerEmail = orderProduct.Order.Buyer.Email,
                        BuyerName = orderProduct.Order.Buyer.Name,
                        OrderId = orderProduct.Order.Id.DisplayShortGuid(),
                        SellerName = orderProduct.Order.Seller.AlternativeName ?? orderProduct.Order.Seller.Name,
                        ProductName = orderProduct.Product.Title,
                        ExtraInfo = orderProduct.Order.ExtraInfo,
                        RentalStartDate = FormatHelper.DisplayDateTime(rentalStartUtc, timeZoneBuyer, cultureBuyer),
                        RentalEndDate = FormatHelper.DisplayDateTime(rentalEndUtc, timeZoneBuyer, cultureBuyer)
                    },
                    MaaSConfig.Current.Environment.Smtp.DefaultFromAddress,
                    new[] { orderProduct.Order.Buyer.Email },
                    orderProduct.Order.Buyer.Lang);

            #endregion Email para comprador

            return OperationResult.Success;
        }
    }
}