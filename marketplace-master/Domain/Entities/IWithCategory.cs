using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithCategory
    {
        IList<Category> Categories { get; }

        int? Category { get; }
    }
}