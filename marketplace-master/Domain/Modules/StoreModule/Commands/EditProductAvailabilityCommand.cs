using Domain.Core;
using Domain.Entities;
using Domain.Modules.StoreModule.Entities;
using Framework;
using System;

namespace Domain.Modules.StoreModule.Commands
{
    public class EditProductAvailabilityCommand : Command
    {
        public Guid AvailabilityId { get; protected set; }

        public int CurrentStock { get; protected set; }

        public int DiscountPercentage { get; protected set; }

        public EditProductAvailabilityCommand(
            ProductAvailability availability, Seller seller,
            int currentStock, int discountPercentage)
            : base(availability.Id.ToString())
        {
            Guard.Ensure.NotNull(availability, nameof(availability));
            Guard.Ensure.NotNull(seller, nameof(seller));
            Guard.Ensure.Rule(() => seller.Id == availability.Product.Store_SellerId, nameof(seller), "Seller invalid");
            Guard.Ensure.Rule(() => currentStock >= 0, nameof(currentStock), "Stock invalid");
            Guard.Ensure.Rule(() => discountPercentage >= 0 && discountPercentage <= 100, nameof(discountPercentage), "Discount invalid");

            var product = availability.Product;

            AvailabilityId = availability.Id;
            CurrentStock = currentStock;
            DiscountPercentage = discountPercentage;

            if (product.IsPackageType && availability.Type == ProductAvailabilityType.Default) DiscountPercentage = 0;
        }
    }
}