using Application.Modules.Portfolio.SubCategories;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Application.Modules.Portfolio
{
    public class PortfolioRequestViewModel
    {
        public string SellerId { get; set; }

        public IList<SubCategoryAndCategorySelector> SubCategories { get; set; }

        public PortfolioRequestViewModel()
        {
            SubCategories = new List<SubCategoryAndCategorySelector>();
        }

        public Domain.Modules.Portfolio.Portfolio Create(IList<SubCategory> subCategories)
        {
            return new Domain.Modules.Portfolio.Portfolio
            {
                SellerId = SellerId,
                SubCategories = subCategories
            };
        }

        public void Update(Domain.Modules.Portfolio.Portfolio portfolio, IList<SubCategory> subCategories)
        {
            portfolio.SubCategories.Clear();

            foreach (var subCategory in subCategories)
                portfolio.SubCategories.Add(subCategory);
        }
    }
}