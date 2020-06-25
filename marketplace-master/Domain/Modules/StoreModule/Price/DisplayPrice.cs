using Application.Config;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Domain.Modules.StoreModule.Price
{
    /// <summary>
    /// Preço do produto para exibição.
    /// </summary>
    public class DisplayPrice
    {
        private decimal _value;

        public decimal Value
        {
            get => _value;
            set
            {
                _value = value;

                var culture = CultureInfo.CurrentUICulture;
                var formatted = _value.ToString("N2", culture);
                var parts = formatted.Split(
                    new[] { culture.NumberFormat.NumberDecimalSeparator },
                    StringSplitOptions.None);

                IntegerPart = parts[0];
                CentsPart = parts[1];
            }
        }

        public bool HasPrice { get; set; }

        public string Currency { get; set; }

        public string IntegerPart { get; set; }

        public string CentsPart { get; set; }

        public string FullPriceText { get; set; }

        public string AdditionalPriceText { get; set; }

        public bool HasAdditionalPrice =>
            !string.IsNullOrWhiteSpace(AdditionalPriceText);

        public bool HasAnyPrice =>
            HasPrice || HasAdditionalPrice;

        public string TextComplementLabel { get; set; }

        public object TextComplementParam { get; set; }

        public bool HasTextComplementLabel =>
            !string.IsNullOrWhiteSpace(TextComplementLabel);

        public bool CanIncrease { get; set; }

        public string PackageCredits { get; set; }

        public string CreditPrice { get; set; }

        public DiscountPrice Discount { get; set; }

        private static decimal CalculateValue(Product product, Price price, IList<RentalShiftDto> shifts)
        {
            var value = price.Value;

            if (ProductPriceSpecs.IsPriceByMinRentalPeriod(product))
            {
                value = value * product.RentalSettings.MinRentalPeriod.Value;
            }

            if (ProductPriceSpecs.IsPriceByMultiplier(product))
            {
                value = value + product.MultiplierPrice.GetValueOrDefault() * product.MinMultiplier.GetValueOrDefault();
            }

            if (ProductPriceSpecs.IsPriceByShifts(product))
            {
                var lesserShift = shifts
                    .OrderBy(shift => shift.DurationMinutes)
                    .FirstOrDefault();

                if (lesserShift != null)
                    value = value * lesserShift.DurationMinutes / 60m;
            }

            return value;
        }

        private static string GetTextComplementLabel(Product product, bool shortPrice)
        {
            if (ProductPriceSpecs.IsPriceByShifts(product))
            {
                return "RENTAL_SHIFT_PRICE_STARTING_AT";
            }

            if (ProductPriceSpecs.IsPriceByRentalUnit(product))
            {
                return product.RentalSettings.GetMinRentalPeriodUnitLabel(true);
            }

            if (ProductPriceSpecs.IsPackageType(product) && !shortPrice)
            {
                return "PRODUCT_PACKAGE_CREDITS_UNIT_DISPLAY";
            }

            if (ProductPriceSpecs.IsPackageType(product) && shortPrice)
            {
                return "PRODUCT_PACKAGE_CREDITS_UNIT";
            }

            return null;
        }

        private static object GetTextComplementParam(Product product, bool shortPrice)
        {
            if (ProductPriceSpecs.IsPackageType(product) && !shortPrice)
            {
                return product.PackageCredits.GetValueOrDefault();
            }

            return null;
        }

        private static string GetPackageCredits(Product product)
        {
            if (ProductPriceSpecs.IsPackageType(product))
            {
                return product.PackageCredits.Value.ToString();
            }

            return string.Empty;
        }

        private static string GetCreditPrice(Product product, DisplayPrice displayPrice)
        {
            if (ProductPriceSpecs.IsPackageType(product))
            {
                var culture = CultureInfo.CurrentUICulture;
                return (displayPrice.Value / product.PackageCredits.Value).ToString("N2", culture);
            }

            return string.Empty;
        }

        private static string GetAdditionalPriceText(Product product, DisplayPrice displayPrice)
        {
            if (ProductPriceSpecs.IsPriceByMultiplier(product))
            {
                return (displayPrice.HasPrice ? "+ " : "") +
                       $"{MaaSConfig.Current.DefaultCurrency} {product.MultiplierPrice.Value.DisplayDecimal(2)} / {GlobalizationHelper.GetString("CART_ADDITIONAL_BASE_UNIT")}";
            }

            return null;
        }

        public static DisplayPrice New(Product product, Price price, bool shortPrice, IList<RentalShiftDto> shifts)
        {
            var displayPrice = new DisplayPrice
            {
                Value = CalculateValue(product, price, shifts),
                Currency = MaaSConfig.Current.DefaultCurrency,
                TextComplementLabel = GetTextComplementLabel(product, shortPrice),
                TextComplementParam = GetTextComplementParam(product, shortPrice),
                CanIncrease = ProductPriceSpecs.CanPriceIncrease(product),
                Discount = DiscountPrice.New(product, price, shortPrice, shifts),
                PackageCredits = GetPackageCredits(product)
            };

            displayPrice.HasPrice = ProductPriceSpecs.HasPrice(displayPrice);

            var culture = CultureInfo.GetCultureInfo(MaaSConfig.Current.Localization.DefaultLang);

            displayPrice.FullPriceText = Math.Round(displayPrice.Value, 2, MidpointRounding.AwayFromZero).DisplayCurrency(culture);
            displayPrice.AdditionalPriceText = GetAdditionalPriceText(product, displayPrice);
            displayPrice.CreditPrice = GetCreditPrice(product, displayPrice);

            return displayPrice;
        }
    }
}