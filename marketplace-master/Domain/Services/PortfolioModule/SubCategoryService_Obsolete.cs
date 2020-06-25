using Domain.Entities.PortfolioModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.SubCategories
{
    [Obsolete]
    public class SubCategoryService_Obsolete : ServiceBase<SubCategory>
    {
        private readonly IRepository<SubCategory> _repository;

        public SubCategoryService_Obsolete(IRepository<SubCategory> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IList<SubCategory> SelectorList(int? categoryId = null, string sellerId = null)
        {
            return _repository.Query()
                .Where(i => !i.Deleted)
                .Where(i => !i.Category.Deleted)
                .Where(i => categoryId == null || i.Category.Id == categoryId)
                .Where(i => sellerId == null || i.Portfolios.Any(j => j.Seller.Id == sellerId))
                .OrderBy(i => i.Name)
                .ToList();
        }
    }
}