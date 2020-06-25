using Application.Search;
using System.Collections.Generic;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Resultado da busca de produtos.
    /// </summary>
    public class SearchResultViewModel : ISearchResult<ProductSearchResultItem>
    {
        public SearchResultViewModel()
        {
            // ResultsOld = new List<ProductWithPrice>();
            Results = new List<ProductSearchResultItem>();
            Facets = new List<FacetGroup>();
        }

        // public IList<ProductWithPrice> ResultsOld { get; set; }
        public IList<ProductSearchResultItem> Results { get; set; }

        public IList<FacetGroup> Facets { get; set; }
        public string NextResults { get; set; }
    }
}