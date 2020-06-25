using System;
using Domain.Core;
using Domain.Entities;
using Domain.Modules.StoreModule.Entities;
using Framework;

namespace Domain.Modules.StoreModule.Commands
{
    public class DeleteProductAvailabilityCommand : Command
    {
        public Guid AvailabilityId { get; set; }

        public string SellerId { get; set; }

        public DeleteProductAvailabilityCommand(ProductAvailability availability, Seller seller)
            : base(availability.Id.ToString())
        {
            Guard.Ensure.NotNull(availability, nameof(availability));
            Guard.Ensure.NotNull(seller, nameof(seller));
            Guard.Ensure.Rule(() => seller.Id == availability.Product.Store_SellerId, nameof(seller), "Seller invalid");

            AvailabilityId = availability.Id;
            SellerId = seller.Id;
        }
    }
}