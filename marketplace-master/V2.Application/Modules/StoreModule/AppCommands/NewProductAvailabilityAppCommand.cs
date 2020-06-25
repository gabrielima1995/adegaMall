using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class NewProductAvailabilityAppCommand
    {
        public string SellerId { get; set; }

        public Guid ProductId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int CurrentStock { get; set; }

        public int DiscountPercentage { get; set; }

        protected NewProductAvailabilityAppCommand()
        {
        }

        public static NewProductAvailabilityAppCommand New(
            string sellerId, Guid productId,
            DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime,
            int currentStock, int discounPercentage)
        {
            return new NewProductAvailabilityAppCommand
            {
                SellerId = sellerId,
                ProductId = productId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                CurrentStock = currentStock,
                DiscountPercentage = discounPercentage
            };
        }
    }
}