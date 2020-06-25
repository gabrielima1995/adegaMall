using Domain.Core;
using Framework;
using System.Linq;

namespace Infrastructure.Data.Context
{
    public static class MessageDispatcherExtensions
    {
        public static void DispatchDomainEvents(this IDomainEventDispatcher eventDispatcher, ScaffoldContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<IEntity>()
                .Where(x => x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(x => x.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                eventDispatcher.Dispatch(domainEvent);
        }
    }
}
