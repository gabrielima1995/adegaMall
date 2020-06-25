using System;

namespace Domain.StoreModule.Entities
{
    public interface IOrderProductRentalInfo
    {
        DateTime? StartDate { get; }

        DateTime? EndDate { get; }

        RentalPeriodUnit? RentalPeriodUnit { get; }

        Guid? ProductAvailability_Id { get; }

        decimal? RentalPeriodValue { get; }

        bool? ChargeDoubleShipping { get; }

        bool? IsReturned { get; }

        DateTime? ReturnedAt { get; }

        bool HasValue { get; }

        void ChangePeriod(DateTime rentalStartUtc, DateTime rentalEndUtc, Guid? productAvailabilityId);

        void SetAsReturned(DateTime? returnedAt = null);
    }
}