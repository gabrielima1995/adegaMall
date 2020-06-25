using System;

namespace Domain.Events.EcommerceHome
{
    public class HomeCategoryAddedEvent : DomainEvent
    {
        public Guid HomeCategoryId { get; private set; }

        public HomeCategoryAddedEvent(Guid homeCategoryId)
        {
            HomeCategoryId = homeCategoryId;
        }
    }
}
