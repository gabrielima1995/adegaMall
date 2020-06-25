using Domain.Core;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System;

namespace Domain.Modules.StoreModule.Commands
{
    public class NewProductAvailabilityCommand : Command
    {
        public Guid AvailabilityId { get; protected set; }

        public Guid ProductId { get; protected set; }

        public DayOfWeek DayOfWeek { get; protected set; }

        public TimeSpan StartTime { get; protected set; }

        public TimeSpan EndTime { get; protected set; }

        public int CurrentStock { get; protected set; }

        public int DiscountPercentage { get; protected set; }

        public NewProductAvailabilityCommand(Guid availabilityId,
            Product product, Seller seller,
            DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime,
            int currentStock, int discountPercentage)
            : base(availabilityId.ToString())
        {
            Guard.Ensure.NotEmpty(availabilityId, nameof(availabilityId));
            Guard.Ensure.NotNull(product, nameof(product));
            Guard.Ensure.NotNull(startTime, nameof(startTime));
            Guard.Ensure.NotNull(endTime, nameof(endTime));
            Guard.Ensure.Rule(() => startTime < endTime, nameof(startTime), "Start date must be greater than end date");
            Guard.Ensure.Rule(() => seller.Id == product.Store_SellerId, nameof(seller), "Seller invalid");
            Guard.Ensure.Rule(() => currentStock >= 0, nameof(currentStock), "Stock invalid");
            Guard.Ensure.Rule(() => discountPercentage >= 0 && discountPercentage <= 100, nameof(discountPercentage), "Discount invalid");

            AvailabilityId = availabilityId;
            ProductId = product.Id;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            CurrentStock = currentStock;
            DiscountPercentage = discountPercentage;

            if (product.IsPackageType) DiscountPercentage = 0;
        }
    }
}