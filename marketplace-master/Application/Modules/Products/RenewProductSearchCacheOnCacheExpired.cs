using Domain.Events;
using System;

namespace Application.Modules.Products
{
    public class RenewProductSearchCacheOnCacheExpired : IDomainEventHandler<CacheExpiredEvent>
    {
        private readonly IProductAppService _productAppService;

        public RenewProductSearchCacheOnCacheExpired(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public void Handle(CacheExpiredEvent domainEvent)
        {
            if (domainEvent.CacheKey != "PRODUCT_SEARCH_CACHE")
                return;

            _productAppService.Search(new Search.SearchRequestViewModel(), null);

            throw new NotImplementedException();
        }
    }
}
