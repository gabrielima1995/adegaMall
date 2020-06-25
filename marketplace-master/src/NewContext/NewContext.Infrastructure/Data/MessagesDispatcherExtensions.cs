using NewContext.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Infrastructure.Data
{
    internal static class MessagesDispatcherExtensions
    {
        public static void DispatchDomainEvents(this IMessagesDispatcher messages, AppDbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.HasDomainEvents)
                .Select(x => x.Entity);

            var tasks = new List<Task>();

            foreach (var entity in entities)
                foreach (var evt in entity.ConsumeDomainEvents())
                    tasks.Add(messages.PublishAsync(evt));

            Task.WaitAll(tasks.ToArray());
        }

        public static async Task DispatchDomainEventsAsync(this IMessagesDispatcher messages, AppDbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.HasDomainEvents)
                .Select(x => x.Entity);

            foreach (var entity in entities)
                foreach (var evt in entity.ConsumeDomainEvents())
                    await messages.PublishAsync(evt);
        }
    }
}
