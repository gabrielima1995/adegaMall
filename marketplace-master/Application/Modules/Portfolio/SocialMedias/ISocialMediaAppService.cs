using Domain.Entities;
using Framework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias
{
    public interface ISocialMediaAppService
    {
        Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo logInfo);

        SellerSocialMedia LinkToSeller(SocialMediaChannelViewModel request);

        void RemoveLinkOfSeller(string sellerId, string provider);
    }
}