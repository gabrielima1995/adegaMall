using System;

namespace Application.Modules.Products.Orders
{
    public class NewRentalProductPeriodViewModel
    {
        public Guid OrderProductId { get; set; }
        public Guid OrderId { get; set; }
        public bool IsFixedRental { get; set; }
        public DateTime? RentalStartDateTime { get; set; }
        public DateTime? RentalEndDateTime { get; set; }
        public Guid? ProductAvailabilityId { get; set; }
        public int Quantity { get; set; }
    }
}