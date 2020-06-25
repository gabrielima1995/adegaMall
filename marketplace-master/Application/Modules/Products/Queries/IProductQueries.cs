using System;
using System.Collections.Generic;

namespace Application.Modules.Products.Queries
{
    public interface IProductQueries
    {
        Guid? GetIdByFriendlyUrl(string friendlyUrl);

        IEnumerable<ProductSelectItem> ListForSelector(string sellerId);
    }
}
