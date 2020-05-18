using Application.Modules.Products;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;

namespace Presentation.Models
{
    public class OrderProductRentalInfoViewModel : IProductDetailsToSelectPeriodViewModel
    {
        public Guid? Id { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public RentalPeriodUnit? RentalPeriodUnit { get; private set; }

        public Guid? ProductAvailabilityId { get; private set; }

        public decimal RentalPeriodValue { get; private set; }

        public bool CanChangePeriodRental { get; private set; }

        public bool ShowChangePeriodOption { get; private set; }

        public string CannotChangePeriodRentalLabel
        {
            get
            {
                switch (OrderProduct.ChangePeriodRentalPolicy)
                {
                    case ChangePeriodRentalPolicy.Disallow:
                        return "CANNOT_CHANGE_PERIOD_RENTAL_DISALLOW_LABEL";

                    case ChangePeriodRentalPolicy.AllowInAdvance:
                        return "CANNOT_CHANGE_PERIOD_RENTAL_ALLOW_IN_ADVANCE_LABEL";

                    case ChangePeriodRentalPolicy.Allow:
                        return "CANNOT_CHANGE_PERIOD_RENTAL_ALLOW_LABEL";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Order Order { get; private set; }

        public OrderProduct OrderProduct { get; private set; }

        public Product Product => OrderProduct.Product;

        public ProductPrice PriceModel => Product.ProductPriceMapped;

        public bool IsFixedRental => Product.RentalSettings.IsFixedRental;

        public int Quantity { get; private set; }

        private OrderProductRentalInfoViewModel()
        {
        }

        private static OrderProductRentalInfoViewModel From(OrderService orderService, Guid? id, Order order, OrderProduct orderProduct, IOrderProductRentalInfo rentalInfo, int quantity)
        {
            var (canChangePeriod, showChangePeriodOption) =
                orderService.CheckIfCanChangePeriodRental(orderProduct, rentalInfo);

            var model = new OrderProductRentalInfoViewModel
            {
                Id = id,
                StartDate = rentalInfo.StartDate.Value,
                EndDate = rentalInfo.EndDate.Value,
                RentalPeriodUnit = rentalInfo.RentalPeriodUnit,
                RentalPeriodValue = rentalInfo.RentalPeriodValue.Value,
                ProductAvailabilityId = rentalInfo.ProductAvailability_Id,
                CanChangePeriodRental = canChangePeriod,
                ShowChangePeriodOption = showChangePeriodOption,
                Order = order,
                OrderProduct = orderProduct,
                Quantity = quantity
            };

            return model;
        }

        public static IEnumerable<OrderProductRentalInfoViewModel> From(OrderService orderService, OrderProduct orderProduct)
        {
            var order = orderProduct.Order;

            if (order.IsRental)
            {
                if (!orderProduct.IsPackageProduct)
                    yield return From(orderService, null, order, orderProduct, orderProduct.RentalInfo, (int)orderProduct.Quantity);
                else foreach (var rentalInfo in orderProduct.RentalsInfo)
                        yield return From(orderService, rentalInfo.Id, order, orderProduct, rentalInfo, 1);
            }
        }
    }
}