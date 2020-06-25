using System.Threading.Tasks;

namespace NewContext.Core
{
    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }
}
