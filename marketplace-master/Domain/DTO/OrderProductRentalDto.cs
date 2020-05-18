using System;

namespace Domain.DTO
{
    public class OrderProductRentalDto
    {
        public Guid OrderId { get; set; }

        public DateTime RentalStartDateTime { get; set; }

        public DateTime RentalEndDateTime { get; set; }

        public decimal Quantity { get; set; }

        public Guid? ProductAvailabilityId { get; set; }

        public bool BlockScheduleForProduct { get; set; }
    }
}