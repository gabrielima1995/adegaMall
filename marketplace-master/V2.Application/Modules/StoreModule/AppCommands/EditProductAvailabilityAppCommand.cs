using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class EditProductAvailabilityAppCommand
    {
        public Guid AvailabilityId { get; set; }

        public string SellerId { get; set; }

        public int CurrentStock { get; set; }

        public int DiscountPercentage { get; set; }

        protected EditProductAvailabilityAppCommand()
        {
        }

        public static EditProductAvailabilityAppCommand New(
            Guid availabilityId, string sellerId,
            int currentStock, int discounPercentage)
        {
            return new EditProductAvailabilityAppCommand
            {
                AvailabilityId = availabilityId,
                SellerId = sellerId,
                CurrentStock = currentStock,
                DiscountPercentage = discounPercentage
            };
        }
    }
}