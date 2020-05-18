using System;
using V2.Application.Modules.Search.Products;
using V2.Application.Modules.Search.Quotes;
using V2.Application.Modules.Search.Sellers;

namespace V2.Application.Modules.Search
{
    public class SearchAppService
    {
        private readonly ISellerSearchService _sellerSearchService;
        private readonly IProductSearchService _productSearchService;
        private readonly IQuoteSearchService _quoteSearchService;

        public SearchAppService(ISellerSearchService sellerSearchService, IProductSearchService productSearchService, IQuoteSearchService quoteSearchService)
        {
            _sellerSearchService = sellerSearchService;
            _productSearchService = productSearchService;
            _quoteSearchService = quoteSearchService;
        }

        public CommonSearchResult Search(CommonSearchRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            switch (request.Type)
            {
                case SearchRequestType.Seller:
                    return _sellerSearchService.Search(request);

                case SearchRequestType.Product:
                    return  _productSearchService.Search(request);

                case SearchRequestType.Portfolio:
                    return _quoteSearchService.Search(request);

                default:
                    throw new ArgumentOutOfRangeException(nameof(request.Type), $"Search request type '{request.Type}' is not recognized.");
            }

            throw new NotImplementedException();
        }
    }
}