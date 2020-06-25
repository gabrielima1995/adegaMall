using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class DeleteProductAvailabilityAppCommand
    {
        public string SellerId { get; set; }

        public Guid AvailabilityId { get; set; }

        public DeleteProductAvailabilityAppCommand(
            string sellerId, Guid availabilityId)
        {
            SellerId = sellerId;
            AvailabilityId = availabilityId;
        }
    }
}