using Domain.Core;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Framework;
using System;

namespace Domain.Modules.StoreModule.Commands
{
    public class NewRecurringProductAvailabilityCommand : Command
    {
        public Guid AvailabilityId { get; protected set; }

        public Guid ProductId { get; protected set; }

        public DayOfWeek[] DaysOfWeek { get; protected set; }

        public RecurringProductAvailabilityUnit Unit { get; protected set; }

        public int UnitValue { get; set; }

        public TimeSpan StartTime { get; protected set; }

        public TimeSpan EndTime { get; protected set; }

        public int CurrentStock { get; protected set; }

        public int DiscountPercentage { get; protected set; }

        public NewRecurringProductAvailabilityCommand(Guid availabilityId,
            Product product, Seller seller,
            DayOfWeek[] daysOfWeek, RecurringProductAvailabilityUnit unit, int unitValue,
            TimeSpan startTime, TimeSpan endTime,
            int currentStock, int discountPercentage)
            : base(availabilityId.ToString())
        {
            Guard.Ensure.NotEmpty(availabilityId, nameof(availabilityId));
            Guard.Ensure.NotNull(product, nameof(product));
            Guard.Ensure.NotNull(startTime, nameof(startTime));
            Guard.Ensure.NotNull(endTime, nameof(endTime));
            Guard.Ensure.Rule(() => startTime < endTime, nameof(startTime), "Start time must be greater than end time");
            Guard.Ensure.Rule(() => seller.Id == product.Store_SellerId, nameof(seller), "Seller invalid");
            Guard.Ensure.Rule(() => currentStock >= 0, nameof(currentStock), "Stock invalid");
            Guard.Ensure.Rule(() => discountPercentage >= 0 && discountPercentage <= 100, nameof(discountPercentage), "Discount invalid");
            Guard.Ensure.Rule(() => unitValue > 0, nameof(unitValue), "Unit value invalid");

            if (unit == RecurringProductAvailabilityUnit.Weeks)
            {
                Guard.Ensure.Rule(() => daysOfWeek != null && daysOfWeek.Length > 0, nameof(daysOfWeek), "DaysOfWeek required");
            }

            AvailabilityId = availabilityId;
            ProductId = product.Id;
            DaysOfWeek = daysOfWeek;
            Unit = unit;
            UnitValue = unitValue;
            StartTime = startTime;
            EndTime = endTime;
            CurrentStock = currentStock;
            DiscountPercentage = discountPercentage;

            if (unit == RecurringProductAvailabilityUnit.Days) DaysOfWeek = Array.Empty<DayOfWeek>();
        }
    }
}