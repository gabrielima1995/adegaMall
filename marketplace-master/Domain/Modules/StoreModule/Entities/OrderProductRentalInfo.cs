using System;

namespace Domain.StoreModule.Entities
{
    public class OrderProductRentalInfo : IOrderProductRentalInfo
    {
        public DateTime? StartDate { get; private set; }

        public DateTime? EndDate { get; private set; }

        public RentalPeriodUnit? RentalPeriodUnit { get; private set; }

        public Guid? ProductAvailability_Id { get; private set; }

        public decimal? RentalPeriodValue { get; private set; }

        public bool? ChargeDoubleShipping { get; private set; }

        public bool? IsReturned { get; private set; }

        public DateTime? ReturnedAt { get; private set; }

        public bool HasValue => StartDate != null && EndDate != null && RentalPeriodUnit != null;

        public OrderProductRentalInfo()
        {
        }

        public OrderProductRentalInfo(CartItemRentalInfo rentalInfo, bool chargeDoubleShipping)
        {
            if (rentalInfo.StartDate >= rentalInfo.EndDate)
                throw new ArgumentException("End date must come after the start date.", nameof(rentalInfo));

            StartDate = rentalInfo.StartDate;
            EndDate = rentalInfo.EndDate;
            RentalPeriodUnit = rentalInfo.RentalPeriodUnit;
            ProductAvailability_Id = rentalInfo.ProductAvailability_Id;
            RentalPeriodValue = rentalInfo.RentalPeriodValue;
            ChargeDoubleShipping = chargeDoubleShipping;

            IsReturned = false;
            ReturnedAt = null;
        }

        public void ChangePeriod(DateTime rentalStartUtc, DateTime rentalEndUtc, Guid? productAvailabilityId)
        {
            if (rentalStartUtc >= rentalEndUtc)
                throw new ArgumentException("End date must come after the start date.");

            StartDate = rentalStartUtc;
            EndDate = rentalEndUtc;
            ProductAvailability_Id = productAvailabilityId;
        }

        public void SetAsReturned(DateTime? returnedAt = null)
        {
            IsReturned = true;
            ReturnedAt = returnedAt ?? DateTime.UtcNow;
        }
    }
}