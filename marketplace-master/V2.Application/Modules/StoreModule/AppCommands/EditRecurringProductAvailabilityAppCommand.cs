using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class EditRecurringProductAvailabilityAppCommand
    {
        public Guid AvailabilityId { get; set; }

        public string SellerId { get; set; }

        public int CurrentStock { get; set; }

        public int DiscountPercentage { get; set; }

        protected EditRecurringProductAvailabilityAppCommand()
        {
        }

        public static EditRecurringProductAvailabilityAppCommand New(
            Guid availabilityId, string sellerId,
            int currentStock, int discounPercentage)
        {
            return new EditRecurringProductAvailabilityAppCommand
            {
                AvailabilityId = availabilityId,
                SellerId = sellerId,
                CurrentStock = currentStock,
                DiscountPercentage = discounPercentage
            };
        }
    }
}