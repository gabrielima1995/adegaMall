using Application.Search;
using System.Collections.Generic;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Resultado da busca de produtos.
    /// </summary>
    public class NearbyProductsResultViewModel : ISearchResult<ProductSearchResultItem>
    {
        public NearbyProductsResultViewModel()
        {
            Results = new List<ProductSearchResultItem>();
        }

        public IList<ProductSearchResultItem> Results { get; set; }

        public string NextResults { get; set; }
    }
}