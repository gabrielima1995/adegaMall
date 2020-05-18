using Domain.Core;
using Domain.Events;
using Infrastructure;
using System;
using System.Linq;
using Unity;

namespace V2.Infrastructure
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ErrorReportingService _errorReportingService;
        private readonly IUnityContainer _container;

        public DomainEventDispatcher(
            ErrorReportingService errorReportingService,
            IUnityContainer container)
        {
            _errorReportingService = errorReportingService;
            _container = container;
        }

        public void Dispatch<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEvent
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _container.ResolveAll(handlerType);

            if (handlers.Any())
            {
                var wrapperType = typeof(DomainEventHandler<>).MakeGenericType(domainEvent.GetType());

                var wrappedHandlers = handlers
                    .Cast<object>()
                    .Select(handler => (DomainEventHandler)Activator.CreateInstance(wrapperType, handler));

                foreach (var handler in wrappedHandlers)
                    SafeHandle(handler, domainEvent);
            }
        }

        private void SafeHandle(DomainEventHandler handler, DomainEvent domainEvent)
        {
            try
            {
                handler.Handle(domainEvent);
            }
            catch (Exception ex)
            {
                _errorReportingService.ReportError(ex);
            }
        }

        private abstract class DomainEventHandler
        {
            public abstract void Handle(DomainEvent domainEvent);
        }

        private class DomainEventHandler<T> : DomainEventHandler where T : DomainEvent
        {
            private readonly IDomainEventHandler<T> _handler;

            public DomainEventHandler(IDomainEventHandler<T> handler)
            {
                _handler = handler;
            }

            public override void Handle(DomainEvent domainEvent)
            {
                _handler.Handle((T)domainEvent);
            }
        }
    }
}
