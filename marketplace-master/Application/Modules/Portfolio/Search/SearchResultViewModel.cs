using Application.Search;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Modules.Portfolio.Search
{
    public class SearchResultViewModel : ISearchResult<Seller>
    {
        public IList<Seller> Results { get; set; }

        public string NextResults { get; set; }
    }
}