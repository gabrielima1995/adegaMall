using Domain.Events;

namespace Application
{
    public class CacheExpiredEvent : DomainEvent
    {
        public string CacheKey { get; private set; }

        protected CacheExpiredEvent(string cacheKey)
        {
            CacheKey = cacheKey;
        }
    }
}
