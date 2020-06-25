using Application.Config;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;

namespace Domain.Modules.StoreModule.Price
{
    public static class ProductPriceSpecs
    {
        private static MaaSConfig Config => MaaSConfig.Current;

        private static bool PriceEnabled() =>
            Config.Product.Price.Enabled;

        private static bool RentalEnabled() =>
            Config.Product.BusinessModel.Rental.Enabled;

        private static bool RentalPackageEnabled() =>
            Config.Product.BusinessModel.Rental.Package.Enabled;

        private static bool RentalPeriodModeUnrestricted() =>
            Config.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Unrestricted;

        private static bool RentalPeriodModeShifts() =>
            Config.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts;

        private static bool IsFormatPriceByRentalPeriodUnit() =>
            Config.Product.BusinessModel.Rental.FormatPriceByRentalPeriodUnit;

        private static bool PriceMultiplierEnabled() =>
            Config.Product.BusinessModel.Rental.EnablePriceMultiplier;

        public static bool AllowsCalculateBasePrice(Product product)
        {
            return RentalEnabled() &&
                   !product.IsPackageType &&
                   product.RentalSettings != null &&
                   product.RentalSettings.IsFixedRental;
        }

        public static bool IsPriceByMinRentalPeriod(Product product)
        {
            return RentalEnabled() &&
                   RentalPeriodModeUnrestricted() &&
                   !product.IsPackageType &&
                   product.RentalSettings != null &&
                   product.RentalSettings.MinRentalPeriod != null &&
                   (product.RentalSettings.IsFixedRental || !IsFormatPriceByRentalPeriodUnit());
        }

        public static bool IsPriceByMultiplier(Product product)
        {
            return RentalEnabled() &&
                   PriceMultiplierEnabled() &&
                   product.MultiplierPrice.GetValueOrDefault() > 0 &&
                   product.MinMultiplier.HasValue;
        }

        public static bool IsPriceByShifts(Product product)
        {
            return RentalEnabled() &&
                   RentalPeriodModeShifts() &&
                   product.RentalSettings != null;
        }

        public static bool IsPriceByRentalUnit(Product product)
        {
            return RentalEnabled() &&
                   RentalPeriodModeUnrestricted() &&
                   !product.IsPackageType &&
                   product.RentalSettings != null &&
                   !(product.RentalSettings.IsFixedRental || !IsFormatPriceByRentalPeriodUnit());
        }

        public static bool IsPackageType(Product product)
        {
            return RentalEnabled() &&
                   RentalPeriodModeUnrestricted() &&
                   RentalPackageEnabled() &&
                   product.IsPackageType;
        }

        public static bool CanPriceIncrease(Product product)
        {
            return RentalEnabled() &&
                   (PriceMultiplierEnabled() && product.MaxMultiplier > product.MinMultiplier ||
                    product.RentalSettings != null && product.RentalSettings.MaxRentalPeriod > product.RentalSettings.MinRentalPeriod);
        }

        public static bool HasPrice(DisplayPrice displayPrice)
        {
            return PriceEnabled() && displayPrice.Value > 0M;
        }

        public static bool MustCalculatePriceAllProducts(MaaSConfig oldConfig, MaaSConfig newConfig)
        {
            return (newConfig.Product.Enabled && !oldConfig.Product.Enabled) ||
                   (newConfig.Product.Price.Enabled && !oldConfig.Product.Price.Enabled) ||
                   (newConfig.Product.BusinessModel.Rental.Package.Enabled != oldConfig.Product.BusinessModel.Rental.Package.Enabled) ||
                   (newConfig.Product.BusinessModel.Rental.RentalPeriodMode != oldConfig.Product.BusinessModel.Rental.RentalPeriodMode) ||
                   (newConfig.Product.BusinessModel.Rental.FormatPriceByRentalPeriodUnit != oldConfig.Product.BusinessModel.Rental.FormatPriceByRentalPeriodUnit) ||
                   (newConfig.Product.BusinessModel.Rental.EnablePriceMultiplier != oldConfig.Product.BusinessModel.Rental.EnablePriceMultiplier);
        }
    }
}