using Domain.Events;
using Domain.Events.Customization;
using Domain.Interfaces;
using Infrastructure.Data.Context;
using System;
using System.Linq;
using V2.Application.Modules.Customization.Queries;

namespace V2.Infrastructure.Modules.Customization.Queries
{
    public class PlatformCustomizationQueries : IPlatformCustomizationQueries,
        IDomainEventHandler<PlatformCustomizationUpdatedEvent>
    {
        private const string CacheKey = "PlatformCustomizationQueries_GetHtmlCustomization";
        private static readonly object _lock = new object();
        private readonly ICache _cache;
        private readonly ScaffoldContext _scaffoldContext;

        public PlatformCustomizationQueries(
            ICache cache,
            ScaffoldContext scaffoldContext)
        {
            _cache = cache;
            _scaffoldContext = scaffoldContext;
        }

        public HtmlCustomization GetHtmlCustomization()
        {
            if (!_cache.Contains(CacheKey))
            {
                lock (_lock)
                {
                    var custom = _scaffoldContext.PlatformCustomization.SingleOrDefault();
                    var dto = new HtmlCustomization
                    {
                        Head = custom?.HtmlHead,
                        BodyStart = custom?.HtmlBodyStart,
                        BodyEnd = custom?.HtmlBodyEnd
                    };

                    _cache.Add(CacheKey, dto, TimeSpan.FromDays(7d), CacheTimeoutType.Sliding);
                }
            }

            return _cache.Get<HtmlCustomization>(CacheKey);
        }

        public void Handle(PlatformCustomizationUpdatedEvent domainEvent)
        {
            _cache.Remove(CacheKey);
        }
    }
}
