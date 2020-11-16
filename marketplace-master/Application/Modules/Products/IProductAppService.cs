using Application.Modules.Products.Search;
using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;

namespace Application.Modules.Products
{
    public interface IProductAppService
    {
        SearchViewModel Search(SearchRequestViewModel request, string buyerId);

        SearchResultViewModel SearchOnlyResults(SearchRequestViewModel request, string buyerId);

        IList<Product> GetRelatedProducts(Guid productId, IEnumerable<Guid> sellerProductsIds);

        IList<Product> GetAnotherProductsTemplate(Guid productId, Guid templateId);

        //ProductPageViewModel GetProductPage(Guid id);
    }
}