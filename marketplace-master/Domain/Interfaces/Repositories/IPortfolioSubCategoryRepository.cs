using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IPortfolioSubCategoryRepository : ICrudRepository<SubCategory>
    {
        List<SubCategory> ListActive(int? categoryId = null);
    }
}
