using Application.Config;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using System;

namespace Presentation.Models
{
    public class ShippingInfosVM
    {
        public Product Product { get; private set; }
        public Address SellerAddress { get; private set; }
        public Address ShippingAddress { get; private set; }

        public decimal ActualNoShippingCost { get; set; }
        public decimal NoShippingCost { get; set; }
        public decimal OnSitePickupCost { get; set; }
        public decimal PrivateFleetCost { get; private set; }
        public decimal PostalExpressCost { get; set; }
        public decimal PostalRegularCost { get; set; }
        public decimal AsapLogCost { get; private set; }
        public decimal TotalExpressCost { get; private set; }
        public decimal MinimumCost { get; private set; } = decimal.MaxValue;
        public int? DeliveryDays { get; private set; }

        public bool ShowActualNoShipping { get; private set; }
        public bool ShowNoShipping { get; private set; }
        public bool ShowOnSitePickup { get; private set; }
        public bool ShowPrivateFleet { get; private set; }
        public bool ShowPostalExpress { get; private set; }
        public bool ShowPostalRegular { get; private set; }
        public bool ShowAsapLog { get; private set; }
        public bool ShowTotalExpress { get; private set; }
        public DateTime? DeliveryDatePac { get; private set; }
        public DateTime? DeliveryDateSedex { get; private set; }
        public DateTime? DeliveryDateAsapLog { get; private set; }
        public DateTime? DeliveryDateTotalExpress { get; private set; }
        public int DeliveryDaysPac { get; private set; }
        public int DeliveryDaysSedex { get; private set; }
        public int DeliveryDaysAsapLog { get; private set; }
        public int DeliveryDaysTotalExpress { get; private set; }
        public bool ShowAnyShipping => ShowActualNoShipping || ShowNoShipping || ShowOnSitePickup || ShowPrivateFleet || ShowPostalExpress || ShowPostalRegular || ShowAsapLog || ShowTotalExpress;
        public bool ShowAnyShippingWithCharge => ShowPrivateFleet || ShowPostalExpress || ShowPostalRegular || ShowAsapLog || ShowTotalExpress;

        public ShippingInfosVM(Product product, Address sellerAddress, Address shippingAddress,
            IShippingMethodAdapter actualNoShippingAdapter,
            IShippingMethodAdapter noShippingAdapter,
            IShippingMethodAdapter onSitePickupAdapter,
            IShippingMethodAdapter privateFleetAdapter,
            IShippingMethodAdapter postalExpressAdapter,
            IShippingMethodAdapter postalRegularAdapter,
            IShippingMethodAdapter asapLogAdapter,
            IShippingMethodAdapter totalExpressAdapter)
        {
            Product = product;
            SellerAddress = sellerAddress;
            ShippingAddress = shippingAddress;

            var config = MaaSConfig.Current;

            if (shippingAddress != null && config.Product.SomeShippingMethodEnabled)
            {
                var chargeDoubleShipping =
                    config.Product.BusinessModel.Rental.Enabled &&
                        (product.RentalSettings.ChargeDoubleShipping ?? false);

                if (config.Product.ActualNoShipping.Enabled && product.ActualNoShippingEnabled)
                {
                    ShowActualNoShipping = false;
                    ActualNoShippingCost = 0M;
                }

                if (config.Product.NoShipping.Enabled && product.NoShippingEnabled)
                {
                    ShowNoShipping = GetShipping(noShippingAdapter, out var noShippingCost, out var deliveryDate, out var _, chargeDoubleShipping);
                    NoShippingCost = noShippingCost;
                }

                if (config.Product.OnSitePickupShipping.Enabled && product.Store.OnSitePickupEnabled)
                {
                    ShowOnSitePickup = GetShipping(onSitePickupAdapter, out var onSitePickupCost, out var deliveryDate, out var _, chargeDoubleShipping);
                    OnSitePickupCost = onSitePickupCost;

                    if (ShowOnSitePickup)
                    {
                        MinimumCost = Math.Min(MinimumCost, OnSitePickupCost);
                    }
                }

                if (config.Product.PrivateFleetShipping.Enabled && product.Store.PrivateFleetEnabled)
                {
                    ShowPrivateFleet = GetShipping(privateFleetAdapter, out var privateFleetCost, out var _, out var _, chargeDoubleShipping);
                    PrivateFleetCost = privateFleetCost;

                    if (ShowPrivateFleet)
                    {
                        MinimumCost = Math.Min(MinimumCost, PrivateFleetCost);
                    }
                }

                if (config.Product.SedexShipping.Enabled && product.Store.SedexEnabled)
                {
                    ShowPostalExpress = GetShipping(postalExpressAdapter, out var postalExpressCost, out var deliveryDate, out var deliveryDaysSedex, chargeDoubleShipping);
                    ShowPostalExpress = ShowPostalExpress && postalExpressCost > 0m;
                    PostalExpressCost = postalExpressCost;
                    DeliveryDateSedex = deliveryDate;
                    DeliveryDaysSedex = deliveryDaysSedex;

                    if (ShowPostalExpress)
                    {
                        MinimumCost = Math.Min(MinimumCost, PostalExpressCost);

                        if (MinimumCost == PostalExpressCost) DeliveryDays = deliveryDaysSedex;
                    }
                }

                if (config.Product.PACShipping.Enabled && product.Store.PacEnabled)
                {
                    ShowPostalRegular = GetShipping(postalRegularAdapter, out var postalRegularCost, out var deliveryDate, out var deliveryDaysPac, chargeDoubleShipping);
                    ShowPostalRegular = ShowPostalRegular && postalRegularCost > 0m;
                    PostalRegularCost = postalRegularCost;
                    DeliveryDatePac = deliveryDate;
                    DeliveryDaysPac = deliveryDaysPac;

                    if (ShowPostalRegular)
                    {
                        MinimumCost = Math.Min(MinimumCost, PostalRegularCost);

                        if (MinimumCost == PostalRegularCost) DeliveryDays = deliveryDaysPac;
                    }
                }

                if (config.Product.AsapLogShipping.Enabled && product.Store.AsapLogEnabled)
                {
                    ShowAsapLog = GetShipping(asapLogAdapter, out var asapLogCost, out var deliveryDate, out var deliveryDaysAsapLog, chargeDoubleShipping) &&
                                  asapLogCost > decimal.Zero;
                    AsapLogCost = asapLogCost;
                    DeliveryDateAsapLog = deliveryDate;
                    DeliveryDaysAsapLog = deliveryDaysAsapLog;

                    if (ShowAsapLog)
                    {
                        MinimumCost = Math.Min(MinimumCost, AsapLogCost);

                        if (MinimumCost == AsapLogCost) DeliveryDays = deliveryDaysAsapLog;
                    }
                }

                if (config.Product.TotalExpressShipping.Enabled && product.Store.TotalExpressEnabled)
                {
                    ShowTotalExpress = GetShipping(totalExpressAdapter, out var totalExpressCost, out var deliveryDate, out var deliveryDaysTotalExpress, chargeDoubleShipping) &&
                                       totalExpressCost > decimal.Zero;
                    TotalExpressCost = totalExpressCost;
                    DeliveryDateTotalExpress = deliveryDate;
                    DeliveryDaysTotalExpress = deliveryDaysTotalExpress;

                    if (ShowTotalExpress)
                    {
                        MinimumCost = Math.Min(MinimumCost, TotalExpressCost);

                        if (MinimumCost == TotalExpressCost) DeliveryDays = deliveryDaysTotalExpress;
                    }
                }
            }
        }

        private bool GetShipping(IShippingMethodAdapter adapter, out decimal cost, out DateTime? deliveryDate, out int deliveryDays, bool chargeDoubleShipping)
        {
            var result = adapter.GetShippingForProduct(new ProductWithQty(Product), SellerAddress, ShippingAddress, chargeDoubleShipping);
            cost = result.OK ? result.Result.TotalShippingPrice : 0M;
            deliveryDays = result.OK ? result.Result.TotalDeliveryDays : 0;

            if (result.OK &&
                (adapter.GetMethod() == ShippingMethod.PostalExpress || adapter.GetMethod() == ShippingMethod.PostalRegular || adapter.GetMethod() == ShippingMethod.AsapLog || adapter.GetMethod() == ShippingMethod.TotalExpress))
                deliveryDate = adapter.CalculateDeliveryDate(result.Result.TotalDeliveryDays);
            else
                deliveryDate = null;
            return result.OK;
        }
    }
}