using Domain.Events.PortfolioModule;
using Domain.Events.SellerProfileUpdated;
using Framework;
using System;

namespace Domain.Modules.Portfolio
{
    public class PortfolioItemService : ServiceBase<PortfolioItem>
    {
        public PortfolioItemService(IRepository<PortfolioItem> repository)
            : base(repository)
        {
        }

        public OperationResult<PortfolioItem> DeletePortfolioItem(Guid portfolioItemId, string sellerId)
        {
            try
            {
                var portfolioItem = Get(i => i.Id == portfolioItemId && i.Portfolio.SellerId == sellerId);
                portfolioItem.Deleted = true;
                portfolioItem.DeletedAt = DateTime.UtcNow;
                return portfolioItem;
            }
            catch (Exception ex)
            {
                return OperationResult<PortfolioItem>.Fail("",ex.Message);
            }
        }
    }
}