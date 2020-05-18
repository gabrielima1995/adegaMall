using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ProductViewModels
{
    public class RecurringProductAvailabilityVM
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public bool DiscountDisabled { get; set; }

        public bool EndTimeDisabled { get; set; }

        public TimeSpan? EndTimeInterval { get; set; }

        [DisplayNameLocalized("RECURRING_PRODUCT_AVAILABILITY_DAYS_OF_WEEK_LABEL")]
        public IList<DayOfWeek> DaysOfWeek { get; set; } = 
            new List<DayOfWeek>(7);

        public RecurringProductAvailabilityUnit Unit { get; set; } = 
            RecurringProductAvailabilityUnit.Weeks;

        [DisplayNameLocalized("RECURRING_PRODUCT_AVAILABILITY_UNIT_LABEL")]
        [RangeLocalized(0, 99)]
        public int UnitValue { get; set; } = 1;

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_START_TIME_LABEL")]
        [DataType(DataType.Time)]
        [RequiredLocalized]
        public TimeSpan? StartTime { get; set; }

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_END_TIME_LABEL")]
        [DataType(DataType.Time)]
        [RequiredLocalized]
        public TimeSpan? EndTime { get; set; }

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_STOCK_LABEL")]
        [RequiredLocalized]
        [RangeLocalized(0, 10000)]
        public int? CurrentStock { get; set; }

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_DISCOUNT_LABEL")]
        [RangeLocalized(0, 99)]
        public int? Discount { get; set; }

        public static RecurringProductAvailabilityVM Prepare(Product product)
        {
            return new RecurringProductAvailabilityVM()
                .WithProduct(product);
        }

        public static RecurringProductAvailabilityVM Prepare(ProductAvailability availability)
        {
            return new RecurringProductAvailabilityVM
            {
                Id = availability.Id,
                DaysOfWeek = availability.Recurring_DaysOfWeek,
                Unit = availability.Recurring_Unit,
                UnitValue = availability.Recurring_UnitValue,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                CurrentStock = availability.CurrentStock,
                Discount = availability.DiscountPercentage
            }
            .WithProduct(availability.Product);
        }

        public RecurringProductAvailabilityVM WithProduct(Product product)
        {
            ProductId = product.Id;
            DiscountDisabled = false;
            EndTimeDisabled = product.RentalSettings.IsFixedRental;
            EndTimeInterval = product.RentalSettings.CalculateEndTime(TimeSpan.Zero);

            return this;
        }
    }
}