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

        public bool ShowActualNoShipping { get; private set; }
        public bool ShowNoShipping { get; private set; }
        public bool ShowOnSitePickup { get; private set; }
        public bool ShowPrivateFleet { get; private set; }
        public bool ShowPostalExpress { get; private set; }
        public bool ShowPostalRegular { get; private set; }
        public DateTime? DeliveryDatePac { get; private set; }
        public DateTime? DeliveryDateSedex { get; private set; }
        public bool ShowAnyShipping => ShowActualNoShipping || ShowNoShipping || ShowOnSitePickup || ShowPrivateFleet || ShowPostalExpress || ShowPostalRegular;
        public bool ShowAnyShippingWithCharge => ShowPrivateFleet || ShowPostalExpress || ShowPostalRegular;

        public ShippingInfosVM(Product product, Address sellerAddress, Address shippingAddress,
            IShippingMethodAdapter actualNoShippingAdapter,
            IShippingMethodAdapter noShippingAdapter,
            IShippingMethodAdapter onSitePickupAdapter,
            IShippingMethodAdapter privateFleetAdapter,
            IShippingMethodAdapter postalExpressAdapter,
            IShippingMethodAdapter postalRegularAdapter)
        {
            Product = product;
            SellerAddress = sellerAddress;
            ShippingAddress = shippingAddress;

            var config = MaaSConfig.Current;

            if (shippingAddress != null && config.Product.ShippingMode == ShippingMode.PerProduct && config.Product.SomeShippingMethodEnabled)
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
                    ShowNoShipping = GetShipping(noShippingAdapter, out var noShippingCost, out var deliveryDate, chargeDoubleShipping);
                    NoShippingCost = noShippingCost;
                }

                if (config.Product.OnSitePickupShipping.Enabled && product.OnSitePickupEnabled)
                {
                    ShowOnSitePickup = GetShipping(onSitePickupAdapter, out var onSitePickupCost, out var deliveryDate, chargeDoubleShipping);
                    OnSitePickupCost = onSitePickupCost;
                }

                if (config.Product.PrivateFleetShipping.Enabled && product.PrivateFleetEnabled)
                {
                    ShowPrivateFleet = GetShipping(privateFleetAdapter, out var privateFleetCost, out var deliveryDate, chargeDoubleShipping);
                    PrivateFleetCost = privateFleetCost;
                }

                if (config.Product.SedexShipping.Enabled && product.SedexEnabled)
                {
                    ShowPostalExpress = GetShipping(postalExpressAdapter, out var postalExpressCost, out var deliveryDate, chargeDoubleShipping);
                    ShowPostalExpress = ShowPostalExpress && postalExpressCost > 0m;
                    PostalExpressCost = postalExpressCost;
                    DeliveryDateSedex = deliveryDate;
                }

                if (config.Product.PACShipping.Enabled && product.PACEnabled)
                {
                    ShowPostalRegular = GetShipping(postalRegularAdapter, out var postalRegularCost, out var deliveryDate, chargeDoubleShipping);
                    ShowPostalRegular = ShowPostalRegular && postalRegularCost > 0m;
                    PostalRegularCost = postalRegularCost;
                    DeliveryDatePac = deliveryDate;
                }
            }
        }

        private bool GetShipping(IShippingMethodAdapter adapter, out decimal cost, out DateTime? deliveryDate, bool chargeDoubleShipping)
        {
            var result = adapter.GetShippingForProduct(new ProductWithQty(Product), SellerAddress, ShippingAddress, chargeDoubleShipping);
            cost = result.OK ? result.Result.TotalShippingPrice : 0M;
            if (adapter.GetMethod() == ShippingMethod.PostalExpress || adapter.GetMethod() == ShippingMethod.PostalRegular)
                deliveryDate = adapter.CalculateDeliveryDate(result.Result.TotalDeliveryDays);
            else
                deliveryDate = null;
            return result.OK;
        }
    }
}