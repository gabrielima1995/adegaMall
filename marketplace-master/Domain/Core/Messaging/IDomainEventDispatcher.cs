using Domain.Events;

namespace Domain.Core
{
    public interface IDomainEventDispatcher
    {
        void Dispatch<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEvent;
    }
}
