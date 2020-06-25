using Domain.Events;
using Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public abstract class BaseEntity : IEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public abstract object GetId();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    public abstract class BaseEntity<T> : BaseEntity, IEntity<T>
    {
        public T Id { get; set; }

        public override object GetId()
        {
            return Id;
        }
    }

    public abstract class SoftDeletableEntity<T> : BaseEntity<T>
    {
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
