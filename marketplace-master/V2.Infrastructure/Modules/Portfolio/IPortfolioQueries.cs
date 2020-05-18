using System;
using Domain.DTO;
using System.Linq;

namespace V2.Infrastructure.Modules.Portfolio
{
    public interface IPortfolioQueries
    {
        IQueryable<SearchResultItem> SearchItems(SearchRequest request);

        Guid? GetIdByFriendlyUrl(string friendlyUrl);
    }
}
