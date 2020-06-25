using Domain.Core;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class RentProductCommand : Command
    {
        public Guid ProductId { get; private set; }

        public DateTime StartDate { get; private set; }

        public long RentalPeriodInHours { get; private set; }

        public DateTime EndDate { get; private set; }

        public RentProductCommand(Guid productId, DateTime startDate, long rentalPeriodInHours) : base(productId.ToString())
        {
            Guard.Ensure.NotEmpty(productId, nameof(productId));
            Guard.Ensure.GreaterThan(startDate, DateTime.UtcNow, nameof(startDate));
            Guard.Ensure.GreaterThan(rentalPeriodInHours, 0, nameof(rentalPeriodInHours));

            ProductId = productId;
            StartDate = startDate;
            RentalPeriodInHours = rentalPeriodInHours;
            EndDate = StartDate.AddHours(rentalPeriodInHours);
        }
    }
}
