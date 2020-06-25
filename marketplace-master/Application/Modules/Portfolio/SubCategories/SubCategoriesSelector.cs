using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoriesSelector : IWithSubCategory
    {
        public IList<SubCategory> SubCategories { get; set; }

        public int? SubCategory { get; set; }
    }
}