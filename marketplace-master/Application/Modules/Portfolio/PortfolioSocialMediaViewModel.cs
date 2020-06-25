using Domain.Entities;

namespace Application.Modules.Portfolio
{
    public class PortfolioSocialMediaViewModel
    {
        public SocialNetwork SocialNetwork { get; set; }

        public SellerSocialMedia SellerSocialMedia { get; set; }

        public bool IsLinked => SellerSocialMedia != null;
    }
}