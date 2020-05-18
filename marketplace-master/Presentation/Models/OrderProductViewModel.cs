using Application.Config;
using Application.Modules.Products;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class OrderProductViewModel : IProductDetailsToSelectPeriodViewModel
    {
        public OrderProduct OrderProduct { get; }

        public RentalPeriodUnit? RentalPeriodUnit => OrderProduct.RentalInfo?.RentalPeriodUnit;

        public MarketplaceTimeZone TimeZone { get; }

        public Guid Id => OrderProduct.Id;

        public Order Order => OrderProduct.Order;

        public Product Product => OrderProduct.Product;

        public ProductPrice PriceModel => Product.ProductPriceMapped;

        public bool ShowChangePeriodRentalPolicy =>
            Order.IsRental && 
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.ChangePeriod.Enabled;

        public ChangePeriodRentalPolicy ChangePeriodRentalPolicy => OrderProduct.ChangePeriodRentalPolicy;

        public string ChangePeriodRentalPolicyLabel => ChangePeriodRentalPolicy.GetDescription();

        public int ChangePeriodRentalPolicyHours =>
            MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.ChangePeriod.BeforeInHours;

        public List<OrderProductRentalInfoViewModel> RentalsInfo { get; }

        public int TotalPossibleRentals => OrderProduct.TotalPossibleRentals;

        public bool CanRegisterNewRental => RentalsInfo.Count < TotalPossibleRentals && Order.Status != OrderStatus.Canceled;

        public string Image => OrderProduct.Image;

        public string FullItemName => OrderProduct.FullItemName;

        public string NoShippingUrl => OrderProduct.NoShippingUrl;

        public int Quantity => (int)OrderProduct.Quantity;

        public decimal Price => OrderProduct.Price;

        public decimal SubTotal => OrderProduct.SubTotal;

        public decimal Shipping => OrderProduct.Shipping;

        public decimal Amount => OrderProduct.Amount;

        public int Multiplier => OrderProduct.Multiplier;

        public decimal MultiplierPrice => OrderProduct.MultiplierPrice;

        public int PackageCredits => OrderProduct.PackageCredits.GetValueOrDefault(0);

        public bool IsFixedRental => Product.RentalSettings.IsFixedRental;

        public bool IsPackageProduct => OrderProduct.IsPackageProduct;

        public OrderProductViewModel(OrderService orderService, OrderProduct orderProduct, MarketplaceTimeZone timeZone)
        {
            OrderProduct = orderProduct;
            TimeZone = timeZone;
            RentalsInfo = OrderProductRentalInfoViewModel.From(orderService, orderProduct)
                .OrderBy(i => i.StartDate)
                .ToList();
        }
    }
}