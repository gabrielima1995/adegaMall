using Domain.Entities.StoreModule;
using System;

namespace Domain.DTO
{
    public class OrderHistoryListing
    {
        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; }
    }
}