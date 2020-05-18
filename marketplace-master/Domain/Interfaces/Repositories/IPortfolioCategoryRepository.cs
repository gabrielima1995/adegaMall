using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IPortfolioCategoryRepository : ICrudRepository<Category>
    {
        List<Category> ListActive();

        List<Category> ListServicedBySeller(string sellerId);
    }
}
