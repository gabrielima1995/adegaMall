using Domain.Entities.StoreModule;
using System;

namespace Domain.Events
{
    public class NewOrderRegisteredEvent : DomainEvent
    {
        public Guid OrderId { get; private set; }
        public string SellerId { get; private set; }
        public bool IsRental { get; }

        public NewOrderRegisteredEvent(Guid orderId, string sellerId, bool isRental)
        {
            OrderId = orderId;
            SellerId = sellerId;
            IsRental = isRental;
        }
    }
}
