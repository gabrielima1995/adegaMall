using Domain.Entities.StoreModule;
using System;

namespace Domain.Events.StoreModule
{
    public class RentalInfoCreated : DomainEvent
    {
        public Guid OrderId { get; }
        public string SellerId { get; }
        public OrderProduct OrderProduct { get; }

        public RentalInfoCreated(Guid orderId, string sellerId, OrderProduct orderProduct)
        {
            OrderId = orderId;
            SellerId = sellerId;
            OrderProduct = orderProduct;
        }
    }
}