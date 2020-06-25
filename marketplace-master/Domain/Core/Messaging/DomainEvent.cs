using System;

namespace Domain.Events
{
    public abstract class DomainEvent : Message
    {
        public DateTime Timestamp { get; }

        protected DomainEvent()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}