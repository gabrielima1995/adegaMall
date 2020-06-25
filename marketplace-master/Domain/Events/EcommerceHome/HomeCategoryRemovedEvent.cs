using System;

namespace Domain.Events.EcommerceHome
{
    public class HomeCategoryRemovedEvent : DomainEvent
    {
        public Guid HomeCategoryId { get; private set; }

        public HomeCategoryRemovedEvent(Guid homeCategoryId)
        {
            HomeCategoryId = homeCategoryId;
        }
    }
}
