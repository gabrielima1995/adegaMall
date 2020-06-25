using Domain.Events;
using Newtonsoft.Json;
using System;

namespace Domain.Core
{
    public class DomainEventHandlerFailedException : Exception
    {
        public DomainEventHandlerFailedException(Type handlerType, DomainEvent domainEvent, string errorMessage)
            : base($"Handler of type '{handlerType.FullName}' failed to handle event. Reason: {errorMessage}")
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(domainEvent);
                base.Data.Add("DomainEventData", jsonData);
            }
            catch { }
        }
    }
}
