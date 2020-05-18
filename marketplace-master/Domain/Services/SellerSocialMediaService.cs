using Domain.Entities;
using Framework;

namespace Domain.Sellers.SocialMedias
{
    public class SellerSocialMediaService : ServiceBase<SellerSocialMedia>
    {
        public SellerSocialMediaService(IRepository<SellerSocialMedia> repository)
            : base(repository)
        {
        }
    }
}