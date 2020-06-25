using System.Collections.Generic;
using System.Linq;

namespace NewContext.Core
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> domainEvents = new List<IDomainEvent>();

        public bool HasDomainEvents => domainEvents.Any();

        public IEnumerable<IDomainEvent> ConsumeDomainEvents()
        {
            var events = domainEvents.ToArray();
            domainEvents.Clear();
            return events;
        }

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return GetId() == other.GetId();
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + GetId()).GetHashCode();
        }

        public abstract object GetId();
    }

    public abstract class Entity<TId> : Entity
    {
        public TId Id { get; protected set; }

        protected Entity(TId id) => Id = id;

        public override object GetId() => Id;
    }
}
