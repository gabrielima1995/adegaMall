using Domain.Events;
using System.Collections.Generic;

namespace Framework
{
    public interface IEntity
    {
        object GetId();

        IReadOnlyCollection<DomainEvent> DomainEvents { get; }

        void AddDomainEvent(DomainEvent domainEvent);

        void ClearDomainEvents();
    }

    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
}