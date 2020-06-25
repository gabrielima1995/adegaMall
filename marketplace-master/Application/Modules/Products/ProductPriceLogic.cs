using Application.Config;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using static Application.Modules.Products.ProductPriceDto;

namespace Application.Modules.Products
{
    //public class ProductPriceDto
    //{
    //    private readonly Product product;

    //    internal ProductPriceDto(Product product, DisplayPriceDto price, DiscountInfoDto discountInfo)
    //    {
    //        this.product = product;
    //        Price = price;
    //        DiscountInfo = discountInfo;
    //    }

    //    public DisplayPriceDto Price { get; }
    //    public DiscountInfoDto DiscountInfo { get; }

    //    public string FormattedAdditionalPrice =>
    //        (HasBasePrice ? "+ " : "") +
    //        $"{MaaSConfig.Current.DefaultCurrency} {product.MultiplierPrice.Value.DisplayDecimal(2)} / {GlobalizationHelper.GetString("CART_ADDITIONAL_BASE_UNIT")}";

    //    public bool HasAnyPrice =>
    //        HasBasePrice || HasAdditionalPrice;

    //    public bool HasBasePrice =>
    //        MaaSConfig.Current.Product.Price.Enabled && Price.HasPrice;

    //    public bool HasAdditionalPrice =>
    //        MaaSConfig.Current.Product.BusinessModel.Rental.EnablePriceMultiplier &&
    //        product.MultiplierPrice.GetValueOrDefault() > 0 && product.MaxMultiplier.HasValue;

    //    public class DisplayPriceDto
    //    {
    //        private DisplayPriceDto(decimal price, string integerPart, string centsPart, string textComplement, bool priceCanIncrease, IEnumerable<string> priceSpecifications, string packageCredits, string creditPrice)
    //        {
    //            Currency = MaaSConfig.Current.DefaultCurrency;
    //            IntegerPart = integerPart;
    //            CentsPart = centsPart;
    //            TextComplement = textComplement;
    //            PriceCanIncrease = priceCanIncrease;
    //            PriceSpecifications = priceSpecifications.ToList();
    //            PackageCredits = packageCredits;
    //            CreditPrice = creditPrice;
    //            HasPrice = price > 0;
    //            FullPriceText = $"{Currency} {price.DisplayDecimal(2)}";
    //        }

    //        internal static DisplayPriceDto Create(Product product, decimal price, string textComplement, bool priceCanIncrease, IEnumerable<string> priceSpecifications)
    //        {
    //            var culture = CultureInfo.CurrentUICulture;
    //            var formatted = price.ToString("N2", culture);
    //            var parts = formatted.Split(
    //                new[] { culture.NumberFormat.NumberDecimalSeparator },
    //                StringSplitOptions.None);

    //            var packageCredits = "";
    //            var creditPrice = "";

    //            if (product.IsPackageType && product.PackageCredits.HasValue)
    //            {
    //                packageCredits = product.PackageCredits.Value.ToString();
    //                creditPrice = (price / product.PackageCredits.Value).ToString("N2", culture);
    //            }

    //            return new DisplayPriceDto(price, parts[0], parts[1], textComplement, priceCanIncrease, priceSpecifications, packageCredits, creditPrice);
    //        }

    //        public string Currency { get; }
    //        public string IntegerPart { get; }
    //        public string CentsPart { get; }
    //        public string TextComplement { get; }
    //        public bool PriceCanIncrease { get; }
    //        public IReadOnlyCollection<string> PriceSpecifications { get; }
    //        public string PackageCredits { get; }
    //        public string CreditPrice { get; set; }
    //        public bool HasPrice { get; }
    //        public string FullPriceText { get; }
    //        public bool HasTextComplement => TextComplement != null;
    //    }

    //    public class DiscountInfoDto
    //    {
    //        private DiscountInfoDto(bool hasDiscount, string formattedDiscountPercent, DisplayPriceDto originalPrice)
    //        {
    //            HasDiscount = hasDiscount;
    //            FormattedDiscountPercent = formattedDiscountPercent;
    //            OriginalPrice = originalPrice;
    //        }

    //        internal static DiscountInfoDto NoDiscount() => new DiscountInfoDto(false, null, null);
    //        internal static DiscountInfoDto With(string formattedDiscountPercent, DisplayPriceDto originalPrice) => new DiscountInfoDto(true, formattedDiscountPercent, originalPrice);

    //        public bool HasDiscount { get; }
    //        public string FormattedDiscountPercent { get; }
    //        public DisplayPriceDto OriginalPrice { get; }
    //    }
    //}

    //public static class ProductPriceLogic
    //{
    //    public static ProductPriceDto GetPriceFor(Product product, IEnumerable<RentalShiftDto> shifts)
    //    {
    //        if (product == null)
    //            throw new ArgumentNullException(nameof(product));

    //        if (shifts == null)
    //            shifts = new List<RentalShiftDto>(0);

    //        var price = DisplayPriceLogic.Create(product, product.GetDisplayPrice().GetValueOrDefault(), shifts);
    //        var discountInfo = DiscountInfoLogic.Create(product, shifts);

    //        return new ProductPriceDto(product, price, discountInfo);
    //    }
    //}

    //internal static class DisplayPriceLogic
    //{
    //    internal static DisplayPriceDto Create(Product product, decimal priceToUse, IEnumerable<RentalShiftDto> productShifts)
    //    {
    //        if (product == null)
    //            throw new ArgumentNullException(nameof(product));

    //        if (productShifts == null)
    //            throw new ArgumentNullException(nameof(productShifts));

    //        var price = CalculateDisplayPrice(product, priceToUse, productShifts);
    //        var textComplement = GenerateTextComplementOrNull(product);
    //        var priceCanIncrease = CheckPriceCanIncrease(product);

    //        return DisplayPriceDto.Create(product, price, textComplement, priceCanIncrease, new string[0]);
    //    }

    //    // DisplayPrice
    //    private static decimal CalculateDisplayPrice(Product product, decimal priceToUse, IEnumerable<RentalShiftDto> shifts)
    //    {
    //        // BasePrice
    //        var price = priceToUse;

    //        if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
    //        {
    //            if (MaaSConfig.Current.Product.BusinessModel.Rental.EnablePriceMultiplier)
    //                price += product.MultiplierPrice.GetValueOrDefault() * product.MinMultiplier.GetValueOrDefault();

    //            if (product.RentalSettings != null)
    //            {
    //                if (MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts)
    //                {
    //                    var lesserShift = shifts.OrderBy(shift => shift.DurationMinutes).FirstOrDefault();
    //                    if (lesserShift != null)
    //                        return price * lesserShift.DurationMinutes / 60m;
    //                }
    //            }
    //        }

    //        return price;
    //    }

    //    private static bool CheckPriceCanIncrease(Product product)
    //    {
    //        if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
    //        {
    //            if (MaaSConfig.Current.Product.BusinessModel.Rental.EnablePriceMultiplier &&
    //                product.MaxMultiplier > product.MinMultiplier)
    //                return true;

    //            if (product.RentalSettings != null &&
    //                product.RentalSettings.MaxRentalPeriod > product.RentalSettings.MinRentalPeriod)
    //                return true;
    //        }

    //        return false;
    //    }

    //    private static string GenerateTextComplementOrNull(Product product)
    //    {
    //        if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled)
    //        {
    //            if (MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts)
    //                return GlobalizationHelper.GetString("RENTAL_SHIFT_PRICE_STARTING_AT");

    //            if (MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Unrestricted)
    //            {
    //                if (!product.RentalSettings.IsFixedRental || MaaSConfig.Current.Product.BusinessModel.Rental.FormatPriceByRentalPeriodUnit)
    //                {
    //                    var label = product.RentalSettings.GetMinRentalPeriodUnitLabel(true);
    //                    return GlobalizationHelper.GetString(label);
    //                }
    //            }

    //            if (MaaSConfig.Current.Product.BusinessModel.Rental.Package.Enabled && product.IsPackageType)
    //                return $"{product.PackageCredits.GetValueOrDefault()} {GlobalizationHelper.GetString("PRODUCT_PACKAGE_CREDITS_UNIT_PLURAL")}";
    //        }

    //        return null;
    //    }
    //}

    //internal static class DiscountInfoLogic
    //{
    //    internal static DiscountInfoDto Create(Product product, IEnumerable<RentalShiftDto> shifts)
    //    {
    //        var hasDiscount = product.Price.HasValue && product.PriceOriginal.HasValue;
    //        if (hasDiscount)
    //        {
    //            var discountAmount = product.PriceOriginal.Value - product.Price.Value;
    //            var discountPercent = Convert.ToInt32(Math.Floor(discountAmount / product.PriceOriginal.Value * 100));
    //            var formattedDiscount = $"{discountPercent}% off";
    //            var originalPrice = DisplayPriceLogic.Create(product, product.GetOriginalDisplayPrice().GetValueOrDefault(), shifts);

    //            return DiscountInfoDto.With(formattedDiscount, originalPrice);
    //        }

    //        return DiscountInfoDto.NoDiscount();
    //    }
    //}
}
