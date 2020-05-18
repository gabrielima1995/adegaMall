using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Domain.Services
{
    public class NicknameGenerator
    {
        private readonly ISellerRepository repository;

        public NicknameGenerator(ISellerRepository repository)
        {
            this.repository = repository;
        }

        public Seller Generate(Seller seller)
        {
            do seller.GenerateNickname();
            while (repository.ExistsByNickname(seller.Nickname, seller.Id));
            return seller;
        }
    }
}