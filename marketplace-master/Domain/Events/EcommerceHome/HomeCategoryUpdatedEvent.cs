using System;

namespace Domain.Events.EcommerceHome
{
    public class HomeCategoryUpdatedEvent : DomainEvent
    {
        public Guid HomeCategoryId { get; private set; }

        public HomeCategoryUpdatedEvent(Guid homeCategoryId)
        {
            HomeCategoryId = homeCategoryId;
        }
    }
}
