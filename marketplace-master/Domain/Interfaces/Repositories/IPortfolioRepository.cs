using Domain.Modules.Portfolio;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Interfaces.Repositories
{
    public interface IPortfolioRepository : ICrudRepository<Portfolio>
    {
        Portfolio GetForPublicPage(string sellerId);

        IQueryable<PortfolioItem> GetQueryable();

        PortfolioItem GetItemForPublicPage(Guid id);

        List<PortfolioItem> GetRelatedItems(Guid portfolioItemId, IEnumerable<Guid> sellerItemsIds);

        bool ExistsByFriendlyUrl(string friendlyUrl, Guid ignoreId);

        OperationResult Update(PortfolioItem entity, bool saveChanges = false);

        Portfolio GetPortfolioForSeller(string sellerId);
    }
}