using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Services.StoreModule;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Shipping;
using V2.Infrastructure.Shipping.Adapters;

namespace Presentation.Models
{
    public class OrderRatingViewModel
    {
        private readonly Order order;

        public OrderRatingViewModel(UserRating userRating, Order order, bool isSeller)
        {
            UserRating = userRating;
            this.order = order;
            IsSeller = isSeller;
        }

        public UserRating UserRating { get; }
        public bool IsSeller { get; }

        public bool Enabled => ((IsSeller && MaaSConfig.Current.Buyer.UserRatings.Enabled) || MaaSConfig.Current.Seller.UserRatings.Enabled) && UserRating != null;
        public bool IsOrderCompleted => order.Status == OrderStatus.Completed;

        public bool CanPostRatingNow
        {
            get
            {
                if (!IsOrderCompleted && UserRating.Rating == null)
                    return false;

                var ratingDeadline = order.Histories.First(h => h.Status == OrderStatus.Completed).OccurredOn.AddDays(MaaSConfig.Current.Ratings.MaxWaitDays);
                return ratingDeadline >= DateTime.Today;
            }
        }
    }

    public class OrderDetailsVM : OrderDetails
    {
        public List<OrderProductViewModel> Products { get; } = new List<OrderProductViewModel>();
        public List<OrderStatus> AvailableStatuses => ListAvailableStatuses();
        public bool IsOrderCompleted => ShippingMethodAdapter.IsOrderCompleted(Order);
        public bool IsShippingOut => ShippingMethodAdapter.GetMethod().IsPhysicalDelivery() ;
        public ShippingMethod ShippingMethod => ShippingMethodAdapter.GetMethod();
        public OperationResult<Uri> TrackingLink => ShippingMethodAdapter.GetTrackingLink(Order.TrackingCode);
        public BuyerProfileVM BuyerVM { get; }

        public bool BuyerInControlOfOrderCompletion =>
            MaaSConfig.Current.Payment.Common.EnableTransactionWarranty &&
            Order.AutoCompletionDate.HasValue && !IsOrderCompleted;

        public DateTime? CompletedDate =>
            IsOrderCompleted
                ? Order.Histories.FirstOrDefault(h => h.Status == OrderStatus.Completed)?.OccurredOn ?? Order.AutoCompletionDate
                : null;

        public bool ShowShipping
        {
            get
            {
                var noShippingCosts = Order.Shipping == 0m;
                var isQuoteBilling = Order.Products.All(item => item.Product.IsBillingProduct.GetValueOrDefault());
                var isActualNoShipping = Order.ShippingMethod == ShippingMethod.ActualNoShipping;
                var hideShipping = (isQuoteBilling || isActualNoShipping) && noShippingCosts;
                return !hideShipping;
            }
        }

        public bool ShowDeliveryDate => Order.DeliveryDate != null && (Order.ShippingMethod == ShippingMethod.PostalExpress || Order.ShippingMethod == ShippingMethod.PostalRegular || Order.ShippingMethod == ShippingMethod.AsapLog || Order.ShippingMethod == ShippingMethod.TotalExpress);

        public string DeliveryDate => Order.DeliveryDate?.ToShortDateString() ?? "";

        public bool ShowOnSitePickupInfo =>
            Order.ShippingMethod == ShippingMethod.OnSitePickup;

        public bool ShowBoletoPaymentInfo =>
            Order.PaymentMethod == PaymentMethod.Boleto &&
            Order.Status == OrderStatus.PendingPayment;

        public bool ShowOfflinePaymentInfo =>
            Order.PaymentMethod == PaymentMethod.Offline &&
            Order.Status != OrderStatus.Canceled &&
            Order.Status != OrderStatus.CompletedPayment &&
            Order.Status != OrderStatus.Rejected;

        public bool ShowInstallmentsCount =>
            Order.Transaction != null &&
            Order.PaymentMethod.In(PaymentMethod.Boleto, PaymentMethod.CreditCard);

        public OrderRatingViewModel UserRating { get; }

        public bool CanCancelRental { get; }

        public bool ShowCancelOption { get; }

        public bool ShowAddressSeller => MaaSConfig.Current.Product.Order.ShowAddressAfterPayment && 
            (Order.PaymentMethod == PaymentMethod.Offline ||
             Order.IsPaid);

        public bool ShowCancelPolicy =>
            Order.IsRental &&
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.Cancel.Enabled;

        public CancelOrderPolicy CancelOrderPolicy
        {
            get
            {
                if (Products.All(i => i.Product.CancelOrderPolicy == CancelOrderPolicy.Disallow))
                    return CancelOrderPolicy.Disallow;

                if (Products.Any(i => i.Product.CancelOrderPolicy == CancelOrderPolicy.AllowInAdvance))
                    return CancelOrderPolicy.AllowInAdvance;

                return CancelOrderPolicy.Allow;
            }
        }

        public string CancelOrderPolicyLabel =>
            CancelOrderPolicy.GetDescription();

        public int CancelOrderPolicyHours =>
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.Cancel.BeforeInHours;

        public string CannotCancelOrderLabel
        {
            get
            {
                switch (CancelOrderPolicy)
                {
                    case CancelOrderPolicy.Disallow:
                        return "CANNOT_CANCEL_ORDER_DISALLOW_LABEL";

                    case CancelOrderPolicy.AllowInAdvance:
                        return "CANNOT_CANCEL_ORDER_ALLOW_IN_ADVANCE_LABEL";

                    case CancelOrderPolicy.Allow:
                        return "CANNOT_CANCEL_ORDER_ALLOW_LABEL";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Quote RelatedQuote { get; }

        public decimal? SellerNetAmount
        {
            get
            {
                if (Order.Transaction == null)
                    return null;
                if (MaaSConfig.Current.Payment.Moip.MarketplaceZeroCommission)
                    return Order.Transaction.Amount - Order.Transaction.GatewayFee;
                return Order.Transaction.Amount - Order.Transaction.StartupFee - (MaaSConfig.Current.Payment.Common.RecipientChargeProcessingFee ? Order.Transaction.GatewayFee : 0);
            }
        }

        public CustomFieldEntityType EntityType => CustomFieldEntityType.CartItem;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public List<CustomFieldValueDto> CustomFields { get; set; } = new List<CustomFieldValueDto>();
        public List<string> CustomFieldsNames { get; set; } = new List<string>();

        public OrderDetailsVM() : base(null, null)
        {
        }

        public OrderDetailsVM(OrderService orderService, OrderDetailsForBuyerAndSellerDTO orderDetails, UserRating userRating, MarketplaceTimeZone timeZone, bool isSeller, BuyerProfileVM buyerProfileVM) : base(orderDetails.Order, orderDetails.ShippingMethodAdapter)
        {
            foreach (var item in orderDetails.Order.Products)
                Products.Add(new OrderProductViewModel(orderService, item, timeZone));

            (CanCancelRental, ShowCancelOption) = orderService.CheckIfCanCancelRental(orderDetails.Order);
            UserRating = new OrderRatingViewModel(userRating, orderDetails.Order, isSeller);
            RelatedQuote = orderDetails.RelatedQuote;
            BuyerVM = buyerProfileVM;
        }

        public bool CanAuthorizePayment()
        {
            switch (Order.Status)
            {
                default:
                    return false;

                case OrderStatus.CompletedPayment:
                case OrderStatus.AwaitingDelivery:
                case OrderStatus.DeliveryOnTheWay:
                case OrderStatus.Delivered:
                case OrderStatus.AwaitingPickup:
                    return true;
            }
        }

        private List<OrderStatus> ListAvailableStatuses()
        {
            if (Order.Status.In(OrderStatus.Completed, OrderStatus.Canceled, OrderStatus.Rejected, OrderStatus.Returned, OrderStatus.Delivered))
                return new List<OrderStatus>();

            var remove = new[]
            {
                OrderStatus.AntifraudRequestPending,
                OrderStatus.AntifraudAwaitingAnalisys,
                OrderStatus.AntifraudApproved,
                OrderStatus.AntifraudDenied,
                OrderStatus.PendingPayment,
                OrderStatus.CompletedPayment
            };

            var list = ShippingMethodAdapter.ListAvailableStatuses(Order.Status)
                .Where(x => !remove.Any(r => x == r))
                .ToList();

            if (Order.PaymentMethod == PaymentMethod.Offline && !list.Contains(OrderStatus.CompletedPayment))
                list.Add(OrderStatus.CompletedPayment);

            if (Order.Status == OrderStatus.AwaitingDelivery || Order.Status == OrderStatus.AwaitingPickup)
                list.Remove(OrderStatus.Returned);

            if (!BuyerInControlOfOrderCompletion && !list.Contains(OrderStatus.Completed))
                list.Add(OrderStatus.Completed);

            return list;
        }
    }
}