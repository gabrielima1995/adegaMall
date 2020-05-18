using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithSubCategory
    {
        IList<SubCategory> SubCategories { get; }

        int? SubCategory { get; }
    }
}