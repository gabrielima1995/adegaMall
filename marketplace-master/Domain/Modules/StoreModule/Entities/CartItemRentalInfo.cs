using Domain.Modules.StoreModule.Entities;
using System;

namespace Domain.StoreModule.Entities
{
    public class CartItemRentalInfo
    {
        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public RentalPeriodUnit? RentalPeriodUnit { get; private set; }

        public decimal? RentalPeriodValue { get; private set; }

        public bool HasValue => StartDate != null && EndDate != null && RentalPeriodUnit != null;

        public string Hash => $"{StartDate?.Ticks ?? 0}-{EndDate?.Ticks ?? 0}";

        public Guid? ProductAvailability_Id { get; private set; }

        private CartItemRentalInfo()
        {
        }

        public CartItemRentalInfo(DateTime startDate, DateTime endDate, RentalPeriodUnit rentalPeriodUnit, ProductAvailability productAvailability)
        {
            if (startDate >= endDate)
                throw new ArgumentException("End date must come after the start date.", nameof(endDate));

            StartDate = startDate;
            EndDate = endDate;
            RentalPeriodUnit = rentalPeriodUnit;
            RentalPeriodValue = CalculateRentalPeriodValue(startDate, endDate, rentalPeriodUnit);
            ProductAvailability_Id = productAvailability?.Id;
        }

        public CartItemRentalInfo(int fixedPeriod, RentalPeriodUnit rentalPeriodUnit)
        {
            RentalPeriodUnit = rentalPeriodUnit;
            RentalPeriodValue = fixedPeriod;
        }

        public static CartItemRentalInfo Empty() => new CartItemRentalInfo();

        private static decimal CalculateRentalPeriodValue(DateTime startDate, DateTime endDate, RentalPeriodUnit rentalPeriodUnit)
        {
            switch (rentalPeriodUnit)
            {
                case Entities.RentalPeriodUnit.Minute:
                    return (decimal)(endDate - startDate).TotalMinutes;

                case Entities.RentalPeriodUnit.Hour:
                    return (decimal)(endDate - startDate).TotalHours;

                case Entities.RentalPeriodUnit.Day:
                    return (decimal)(endDate - startDate).TotalDays;

                case Entities.RentalPeriodUnit.Month:
                    var monthEnd = endDate.Year * 12 + endDate.Month;
                    var monthStart = startDate.Year * 12 + startDate.Month;
                    return monthEnd - monthStart;

                default:
                    throw new NotImplementedException($"Rental period {rentalPeriodUnit} is not handled.");
            }
        }
    }
}