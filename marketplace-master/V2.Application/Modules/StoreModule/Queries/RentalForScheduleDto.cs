using Domain.Entities.StoreModule;
using System;

namespace V2.Application.Modules.StoreModule.Queries
{
    public class RentalForScheduleDto
    {
        public Guid Id { get; set; }

        public string BuyerName { get; set; }
        public string SellerName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public decimal Quantity { get; set; }

        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public Guid? ProductAvailabilityId { get; set; }

        public bool IsReturned { get; set; }

        public OrderStatus Status { get; set; }
    }
}