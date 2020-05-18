using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ProductViewModels
{
    public class ProductAvailabilityVM
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public bool DiscountDisabled { get; set; }

        public bool EndTimeDisabled { get; set; }

        public TimeSpan? EndTimeInterval { get; set; }

        [DisplayNameLocalized("PRODUCT_AVAILABILITY_DAY_OF_WEEK_LABEL")]
        public DayOfWeek DayOfWeek { get; set; }

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

        public static ProductAvailabilityVM Prepare(Product product)
        {
            return new ProductAvailabilityVM()
                .WithProduct(product);
        }

        public static ProductAvailabilityVM Prepare(ProductAvailability availability)
        {
            return new ProductAvailabilityVM
            {
                Id = availability.Id,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                CurrentStock = availability.CurrentStock,
                Discount = availability.DiscountPercentage
            }
            .WithProduct(availability.Product);
        }

        public ProductAvailabilityVM WithProduct(Product product)
        {
            ProductId = product.Id;
            DiscountDisabled = product.IsPackageType;
            EndTimeDisabled = product.RentalSettings.IsFixedRental;
            EndTimeInterval = product.RentalSettings.CalculateEndTime(TimeSpan.Zero);

            return this;
        }
    }
}