using Application.Config;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Domain.Services
{
    public class AlternativeNameGenerator
    {
        private readonly ISellerRepository repository;

        public AlternativeNameGenerator(ISellerRepository repository)
        {
            this.repository = repository;
        }

        public Seller Generate(Seller seller)
        {
            if (MaaSConfig.Current.Seller.AlternativeName.GenerateEncoded)
            {
                do seller.GenerateAlternativeName();
                while (repository.ExistsByAlternativeNameEncoded(seller.AlternativeName, seller.Id));
            }

            return seller;
        }
    }
}