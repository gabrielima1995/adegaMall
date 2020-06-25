using Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Portfolio.SocialMedias
{
    public class SocialMediaChannelViewModel
    {
        [RequiredLocalized]
        public string Provider { get; set; }

        [RequiredLocalized]
        public string Id { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("NICKNAME_LABEL")]
        public string UserName { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("LINK_LABEL")]
        public string Link { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("FOLLOWERS_LABEL")]
        public int Followers { get; set; }

        public string SellerId { get; set; }

        public SocialMediaChannelViewModel()
        {
        }

        public SocialMediaChannelViewModel(string provider, string id, string userName, string link, int followers)
        {
            Provider = provider;
            Id = id;
            UserName = userName;
            Link = link;
            Followers = followers;
        }

        public SellerSocialMedia CreateSellerSocialMedia(Seller seller, SocialNetwork socialNetwork)
        {
            return new SellerSocialMedia
            {
                ResourceId = Id,
                Nickname = UserName,
                Url = Link,
                Followers = Followers,
                UpdatedAt = DateTime.UtcNow,
                Portfolio = seller.Portfolio,
                SocialNetwork = socialNetwork
            };
        }

        public void UpdateSellerSocialMedia(SellerSocialMedia sellerSocialMedia)
        {
            sellerSocialMedia.ResourceId = Id;
            sellerSocialMedia.Nickname = UserName;
            sellerSocialMedia.Url = Link;
            sellerSocialMedia.Followers = Followers;
            sellerSocialMedia.UpdatedAt = DateTime.UtcNow;
        }
    }
}