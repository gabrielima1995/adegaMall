using Domain.Modules.StoreModule.Entities;
using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewRecurringProductAvailabilityAppCommand
    {
        public string SellerId { get; set; }

        public Guid ProductId { get; set; }

        public DayOfWeek[] DaysOfWeek { get; set; }

        public RecurringProductAvailabilityUnit Unit { get; set; }

        public int UnitValue { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int CurrentStock { get; set; }

        public int DiscountPercentage { get; set; }

        protected NewRecurringProductAvailabilityAppCommand()
        {
        }

        public static NewRecurringProductAvailabilityAppCommand New(
            string sellerId, Guid productId,
            DayOfWeek[] daysOfWeek, RecurringProductAvailabilityUnit unit, int unitValue,
            TimeSpan startTime, TimeSpan endTime,
            int currentStock, int discounPercentage)
        {
            return new NewRecurringProductAvailabilityAppCommand
            {
                SellerId = sellerId,
                ProductId = productId,
                DaysOfWeek = daysOfWeek,
                Unit = unit,
                UnitValue = unitValue,
                StartTime = startTime,
                EndTime = endTime,
                CurrentStock = currentStock,
                DiscountPercentage = discounPercentage
            };
        }
    }
}