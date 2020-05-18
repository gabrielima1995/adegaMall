using Application.Modules.Portfolio;
using Domain.Modules.Portfolio;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models.PortfolioViewModels
{
    public class PortfolioItemPageVM : PortfolioItemVM
    {
        public double SellerRatingAverage { get; set; }
        public int SellerRatingsCount { get; set; }

        public IList<PortfolioItemVM> RelatedItems { get; private set; }

        public PortfolioItemPageVM(PortfolioItem portfolioItem, List<PortfolioItem> relatedItems)
            : base(portfolioItem)
        {
            RelatedItems = relatedItems.Select(i => new PortfolioItemVM(i)).ToList();
        }
    }
}