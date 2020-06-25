using CSharpFunctionalExtensions;
using NewContext.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace NewContext.Infrastructure
{
    public class MessagesDispatcher : IMessagesDispatcher
    {
        private readonly IUnityContainer container;

        public MessagesDispatcher(IUnityContainer container)
        {
            this.container = container;
        }

        public async Task<Result> CommandAsync(ICommand command)
        {
            var type = typeof(ICommandHandler<>);
            var typeArgs = new Type[] { command.GetType() };
            var handlerType = type.MakeGenericType(typeArgs);

            dynamic handler = container.Resolve(handlerType);
            Task<Result> result = handler.HandleAsync((dynamic)command);

            return await result.ConfigureAwait(false);
        }

        public async Task PublishAsync(IDomainEvent domainEvent)
        {
            var type = typeof(IDomainEventHandler<>);
            var typeArgs = new Type[] { domainEvent.GetType() };
            var handlerType = type.MakeGenericType(typeArgs);

            IEnumerable<dynamic> handlers = container.ResolveAll(handlerType);
            foreach (var handler in handlers)
            {
                Task t = handler.HandleAsync((dynamic)domainEvent);
                await t.ConfigureAwait(false);
            }
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var type = typeof(IQueryHandler<,>);
            var typeArgs = new Type[] { query.GetType(), typeof(TResult) };
            var handlerType = type.MakeGenericType(typeArgs);

            dynamic handler = container.Resolve(handlerType);
            Task<TResult> result = handler.HandleAsync((dynamic)query);

            return await result.ConfigureAwait(false);
        }
    }
}
