using System;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;

namespace Domain.Services
{
    public class FriendlyUrlGenerator
    {
        private readonly ISellerRepository sellerRepository;
        private readonly IPortfolioRepository portfolioRepository;

        public FriendlyUrlGenerator(
            ISellerRepository sellerRepository,
            IPortfolioRepository portfolioRepository)
        {
            this.sellerRepository = sellerRepository;
            this.portfolioRepository = portfolioRepository;
        }

        public Seller Generate(Seller seller)
        {
            seller.GenerateFriendlyUrl();

            while (sellerRepository.ExistsByFriendlyUrl(seller.FriendlyUrl, seller.Id))
            {
                var old = seller.FriendlyUrl;

                seller.GenerateFriendlyUrl(seller.Id.Substring(0, 6));

                if (old == seller.FriendlyUrl)
                {
                    seller.GenerateFriendlyUrl(Guid.NewGuid().Substring(6));
                }
            }

            return seller;
        }

        public PortfolioItem Generate(PortfolioItem item)
        {
            item.GenerateFriendlyUrl();

            while (portfolioRepository.ExistsByFriendlyUrl(item.FriendlyUrl, item.Id))
            {
                var old = item.FriendlyUrl;

                item.GenerateFriendlyUrl(item.Id.ToString().Substring(0, 6));

                if (old == item.FriendlyUrl)
                {
                    item.GenerateFriendlyUrl(Guid.NewGuid().Substring(6));
                }
            }

            return item;
        }
    }
}