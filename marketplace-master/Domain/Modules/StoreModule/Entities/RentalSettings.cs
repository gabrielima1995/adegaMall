using Application.Config;
using Domain.Config.Modules.Product;
using Domain.Core;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Entities
{
    public class RentalSettings : ValueObject
    {
        private RentalPeriodUnit? rentalPeriodUnit;

        public int? MinRentalPeriod { get; set; }
        public int? MaxRentalPeriod { get; set; }
        public int? MinRentalNotice { get; set; }
        public RentalPeriodUnit? MinRentalNoticeUnit { get; set; }
        public int? MinRentalMargin { get; set; }
        public RentalPeriodUnit? RentalMarginUnit { get; set; }
        public decimal? MinRentalAmount { get; set; }
        public string AvailabilityWarning { get; set; }
        public bool? ChargeDoubleShipping { get; set; }
        public bool BlockScheduleForProduct { get; set; }

        public RentalPeriodUnit? RentalPeriodUnit
        {
            get
            {
                if (MaaSConfig.Current.Product.BusinessModel.Rental.EnablePeriodPerProduct)
                    return rentalPeriodUnit = rentalPeriodUnit ?? MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;

                return MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;
            }
            set => rentalPeriodUnit = value;
        }

        public BusinessModelConfig_RentalPeriodInHours GetMinRentalPeriodInHours()
        {
            return !MinRentalPeriod.HasValue
                ? BusinessModelConfig_RentalPeriodInHours.Zero
                : MaaSConfig.Current.Product.BusinessModel.Rental.ConvertRentalPeriodInHours(RentalPeriodUnit.Value, MinRentalPeriod.Value);
        }

        public BusinessModelConfig_RentalPeriodInHours GetMaxRentalPeriodInHours()
        {
            return !MaxRentalPeriod.HasValue
                ? BusinessModelConfig_RentalPeriodInHours.Zero
                : MaaSConfig.Current.Product.BusinessModel.Rental.ConvertRentalPeriodInHours(RentalPeriodUnit.Value, MaxRentalPeriod.Value);
        }

        public BusinessModelConfig_RentalPeriodInHours GetMinRentalNoticeInHours()
        {
            return !MinRentalNotice.HasValue
                ? BusinessModelConfig_RentalPeriodInHours.Zero
                : MaaSConfig.Current.Product.BusinessModel.Rental.ConvertRentalPeriodInHours(MinRentalNoticeUnit.Value, MinRentalNotice.Value);
        }

        public BusinessModelConfig_RentalPeriodInHours GetRentalMarginInHours()
        {
            if (!MinRentalMargin.HasValue || !RentalMarginUnit.HasValue) return BusinessModelConfig_RentalPeriodInHours.Zero;

            return MaaSConfig.Current.Product.BusinessModel.Rental.ConvertRentalPeriodInHours(RentalMarginUnit.Value, MinRentalMargin.Value);
        }

        public bool IsFixedRental =>
            MaaSConfig.Current.Product.BusinessModel.Rental.Enabled &&
            MinRentalPeriod.HasValue && MaxRentalPeriod.HasValue && MinRentalPeriod == MaxRentalPeriod;

        private string GetRentalPeriodUnitLabel(RentalPeriodUnit? unit)
        {
            if (unit == null)
                unit = MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodUnit;

            switch (unit)
            {
                case Entities.RentalPeriodUnit.Minute:
                    return "RENTAL_TIMESPAN_MINUTE";

                case Entities.RentalPeriodUnit.Hour:
                    return "RENTAL_TIMESPAN_HOUR";

                case Entities.RentalPeriodUnit.Day:
                    return "RENTAL_TIMESPAN_DAY";

                case Entities.RentalPeriodUnit.Month:
                    return "RENTAL_TIMESPAN_MONTH";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsPlural(int? value, bool onlySingular)
        {
            return !onlySingular && value.GetValueOrDefault() != 1;
        }

        public string GetMinRentalPeriodUnitLabel(bool onlySingular)
        {
            var label = GetRentalPeriodUnitLabel(RentalPeriodUnit);

            if (IsPlural(MinRentalPeriod, onlySingular))
                label += "_PLURAL";

            return label;
        }

        public string GetMinRentalNoticeUnitLabel(bool onlySingular)
        {
            var label = GetRentalPeriodUnitLabel(MinRentalNoticeUnit);

            if (IsPlural(MinRentalNotice, onlySingular))
                label += "_PLURAL";

            return label;
        }

        public string GetMinRentalMarginUnitLabel(bool onlySingular)
        {
            var label = GetRentalPeriodUnitLabel(RentalMarginUnit);

            if (IsPlural(MinRentalMargin, onlySingular))
                label += "_PLURAL";

            return label;
        }

        public DateTime? CalculateEndDateTime(DateTime? startDateTime)
        {
            if (!startDateTime.HasValue) return default(DateTime?);

            if (!IsFixedRental) return default(DateTime?);

            switch (RentalPeriodUnit)
            {
                case Entities.RentalPeriodUnit.Minute:
                    return startDateTime.Value.AddMinutes(MinRentalPeriod.Value);

                case Entities.RentalPeriodUnit.Hour:
                    return startDateTime.Value.AddHours(MinRentalPeriod.Value);

                case Entities.RentalPeriodUnit.Day:
                    return startDateTime.Value.AddDays(MinRentalPeriod.Value);

                case Entities.RentalPeriodUnit.Month:
                    return startDateTime.Value.AddMonths(MinRentalPeriod.Value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(RentalPeriodUnit));
            }
        }

        public TimeSpan? CalculateEndTime(TimeSpan? startTime)
        {
            if (!startTime.HasValue) return default(TimeSpan?);

            if (!IsFixedRental) return default(TimeSpan?);

            switch (RentalPeriodUnit)
            {
                case Entities.RentalPeriodUnit.Minute:
                    return startTime.Value.Add(TimeSpan.FromMinutes(MinRentalPeriod.Value));

                case Entities.RentalPeriodUnit.Hour:
                    return startTime.Value.Add(TimeSpan.FromHours(MinRentalPeriod.Value));

                default:
                    throw new ArgumentOutOfRangeException(nameof(RentalPeriodUnit));
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return MinRentalPeriod.GetValueOrDefault();
            yield return MaxRentalPeriod ?? int.MaxValue;
            yield return MinRentalNotice.GetValueOrDefault();
            yield return MinRentalNoticeUnit;
        }
    }
}