namespace Domain.Events
{
    public interface IDomainEventHandler<TEvent> where TEvent : DomainEvent
    {
        void Handle(TEvent domainEvent);
    }
}
