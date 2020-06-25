using Application.Modules.Portfolio.SocialMedias.Providers;
using Domain.Entities;
using Domain.Sellers.SocialMedias;
using Framework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Modules.Portfolio.SocialMedias
{
    public class SocialMediaAppService : ISocialMediaAppService
    {
        private readonly SellerSocialMediaService _sellerSocialMediaService;
        private readonly IRepository<SocialNetwork> _socialNetworkRepository;
        private readonly IRepository<Seller> _sellerRepository;

        public SocialMediaAppService(
            SellerSocialMediaService sellerSocialMediaService,
            IRepository<SocialNetwork> socialNetworkRepository,
            IRepository<Seller> sellerRepository)
        {
            _sellerSocialMediaService = sellerSocialMediaService;
            _socialNetworkRepository = socialNetworkRepository;
            _sellerRepository = sellerRepository;
        }

        private static ISocialMediaProvider GetSocialMediaProvider(string provider)
        {
            switch (provider.ToLower())
            {
                case "facebook":
                    return new SocialMediaFacebook();

                case "instagram":
                    return new SocialMediaInstagram();

                case "twitter":
                    return new SocialMediaTwitter();

                case "googleplus":
                    return new SocialMediaYoutube();

                case "twitch":
                    return new SocialMediaTwitch();

                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<OperationResult<IList<SocialMediaChannelViewModel>>> GetChannelsAsync(ExternalLoginInfo logInfo)
        {
            return await GetSocialMediaProvider(logInfo.Login.LoginProvider).GetChannelsAsync(logInfo);
        }

        public SellerSocialMedia LinkToSeller(SocialMediaChannelViewModel request)
        {
            var socialNetwork = _socialNetworkRepository.Get(i => i.Name == request.Provider);
            var seller = _sellerRepository.Get(i => i.Id == request.SellerId,
                i => i.Portfolio);

            var sellerSocialMedia = _sellerSocialMediaService.Save(
                get: i => i.Portfolio.SellerId == request.SellerId && i.SocialNetwork.Name == request.Provider,
                create: () => request.CreateSellerSocialMedia(seller, socialNetwork),
                update: request.UpdateSellerSocialMedia);

            return sellerSocialMedia;
        }

        public void RemoveLinkOfSeller(string sellerId, string provider)
        {
            _sellerSocialMediaService.Delete(
                i => i.Portfolio.Seller.Id == sellerId && i.SocialNetwork.Name == provider);
        }
    }
}