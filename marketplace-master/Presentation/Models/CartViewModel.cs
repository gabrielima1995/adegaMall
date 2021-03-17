using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using Presentation.Helpers;
using Presentation.Models.ProductViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;

namespace Presentation.Models
{
    public class CartViewModel
    {
        private readonly Cart cart;
        private readonly List<string> errors;
        private readonly ViewDataDictionary viewData;

        private static readonly string currencySymbol = MaaSConfig.Current.DefaultCurrency;

        public CartViewModel(List<Address> shippingAddresses, Cart cart, ViewDataDictionary viewData, CartWithExtraInfo cartWithExtraInfo)
        {
            this.cart = cart;
            ShippingAddresses = AddShippingAddressList(shippingAddresses, cart);
            this.viewData = viewData;
            CartWithExtraInfo = cartWithExtraInfo;
            errors = CartHelper.GetCartErrors(viewData.ModelState, excludeProductErrors: true).ToList();
            NewShippingAddress = new AddressRegisterModel();
            AvailableShippingMethods = cart.AvailableShippingsMethods;
        }

        private List<Address> AddShippingAddressList(List<Address> shippingAddresses, Cart cart)
        {
            if (cart.ShippingAddress != null && !shippingAddresses.Contains(cart.ShippingAddress))
            {
                shippingAddresses.Add(cart.ShippingAddress);
            }
            return shippingAddresses;
        }

        public IReadOnlyList<ShippingMethod> AvailableShippingMethods { get; }

        public IReadOnlyList<string> CartErrors => errors;

        public IReadOnlyList<CartItem> CartItems
        {
            get
            {
                if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
                    return cart.Items.OrderBy(item => item.RentalInfo.StartDate).ToList();

                return cart.Items.ToList();
            }
        }

        public string FormattedCartItemsTotal => $"{currencySymbol} {cart.SubTotal.DisplayDecimal(2)}";

        public string FormattedCartDiscount => cart.Discount == 0m ? "-" : $"-{currencySymbol} {cart.Discount.DisplayDecimal(2)}";

        public string FormattedCartShipping => cart.ShippingTotal == 0m ? "-" : $"{currencySymbol} {cart.ShippingTotal.DisplayDecimal(2)}";

        public string FormattedCartTotal => $"{currencySymbol} {cart.Amount.DisplayDecimal(2)}";

        public string CartDeliveryDays => cart.DeliveryDate?.ToShortDateString() ?? "";

        public int? TotalDeliveryDays => cart.ShippingMode == ShippingMode.PerStore
            ? cart.Sellers.Max(x => x.SellerDeliveryDays)
            : cart.StockAndShippingValidation.Result?.TotalDeliveryDays;

        public bool HasCartErrors => errors.Any();

        public bool HasProducts => !cart.IsEmpty;

        public bool HasValidationErrors => !viewData.ModelState.IsValid;

        public bool OnlyOneShippingMethodEnabled => MaaSConfig.Current.Product.OnlyOneShippingMethodEnabled;

        public ShippingMethod SelectedShippingMethod => cart.ShippingMethod;

        public IList<Address> ShippingAddresses { get; set; }

        public AddressRegisterModel NewShippingAddress { get; set; }

        public bool IsShippingOut => HasProducts && (
            SelectedShippingMethod == ShippingMethod.AsapLog ||
            SelectedShippingMethod == ShippingMethod.TotalExpress ||
            SelectedShippingMethod == ShippingMethod.PostalRegular ||
            SelectedShippingMethod == ShippingMethod.PostalExpress ||
            SelectedShippingMethod == ShippingMethod.PrivateFleet);

        public string ShippingWarningMessage
        {
            get
            {
                if (cart.StockAndShippingValidation.Result != null)
                {
                    return cart.StockAndShippingValidation.Result
                        .Where(item =>
                            item.ResultType == CorreiosResultType.Warning || (
                            item.ResultType == CorreiosResultType.CommunicationError && !item.IsDefaultShipping))
                        .Select(item => $"{item.Message} ({item.CodeResult})")
                        .FirstOrDefault();
                }
                return null;
            }
        }

        public Cart GetCart() => cart;

        public bool HasCoupon => cart.Coupon != null;

        public string CouponCode => cart.Coupon?.Code ?? string.Empty;

        public IReadOnlyList<string> ListProductsNotSupporting(ShippingMethod method) => cart.Items.Where(item => !item.Product.SupportsShippingMethod(method)).Select(item => item.Product.Title).ToList();

        public VisualizationSettings Visualization => new VisualizationSettings(this);

        public CartWithExtraInfo CartWithExtraInfo { get; }

        public CustomFieldEntityType EntityType => CustomFieldEntityType.CartItem;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }

        public List<CustomFieldValueDto> CustomFields { get; set; } = new List<CustomFieldValueDto>();

        public List<string> CustomFieldsNames { get; set; } = new List<string>();

        public class VisualizationSettings
        {
            private readonly CartViewModel model;

            public VisualizationSettings(CartViewModel model)
            {
                this.model = model;
            }

            public bool ShowShippingAmount
            {
                get
                {
                    var noShippingCosts = model.CartItems.Sum(item => item.Shipping).Equals(0m);
                    var isQuoteBilling = MaaSConfig.Current.Quote.Enabled && MaaSConfig.Current.Quote.QuoteBilling.Enabled && model.CartItems.All(item => item.Product.IsBillingProduct.GetValueOrDefault());
                    var isActualNoShipping = model.SelectedShippingMethod == ShippingMethod.ActualNoShipping;
                    var hideShipping = (isQuoteBilling || isActualNoShipping) && noShippingCosts;

                    return !hideShipping;
                }
            }

            public bool ShowShippingMethods => MaaSConfig.Current.Product.SomeShippingMethodEnabled && !MaaSConfig.Current.Product.OnlyOneShippingMethodEnabled;

            public bool ShouldShowDeliveryTimeMessageFor(ShippingMethod method)
            {
                if (model.SelectedShippingMethod != method)
                    return false;

                var cfg = MaaSConfig.Current.Product;

                switch (method)
                {
                    case ShippingMethod.NoShipping:
                        return cfg.NoShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.ActualNoShipping:
                        return cfg.ActualNoShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.PrivateFleet:
                        return cfg.PrivateFleetShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.OnSitePickup:
                        return cfg.OnSitePickupShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.PostalExpress:
                        return cfg.SedexShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.PostalRegular:
                        return cfg.PACShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.AsapLog:
                        return cfg.AsapLogShipping.DeliveryTimeMessage.Enabled;
                    case ShippingMethod.TotalExpress:
                        return cfg.TotalExpressShipping.DeliveryTimeMessage.Enabled;
                    default:
                        throw new Exception($"Shipping method {method} is not recognized.");
                }
            }

        }
    }
}