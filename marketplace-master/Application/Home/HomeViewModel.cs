using Application.Config;
using Application.Helpers;
using Application.Modules.Products.Search;
using System;
using System.Collections.Generic;

namespace Application.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            BestSellers = new List<BestSellerDto>();
        }

        public IReadOnlyCollection<BestSellerDto> BestSellers { get; set; }
        public SellersSearchHomeViewModel SellersSearch { get; set; } = new SellersSearchHomeViewModel();
        public ProductsSearchHomeViewModel ProductsSearch { get; set; } = new ProductsSearchHomeViewModel();
    }

    public class BestSellerDto
    {
        public BestSellerDto(string id, string name, string description, string avatar, string publicProfileUrl, double averageRating, int reviewsCount)
        {
            Id = id;
            Name = name;
            Description = description;
            Avatar = avatar;
            PublicProfileUrl = publicProfileUrl;
            AverageRating = averageRating;
            ReviewsCount = reviewsCount;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Avatar { get; }
        public string PublicProfileUrl { get; }
        public double AverageRating { get; }
        public int ReviewsCount { get; }

        public string GetScaledImageUrl()
        {
            try
            {
                foreach (var type in new[] { "userimage", "images" })
                {
                    if (Avatar.Contains($"/{type}/") && !Avatar.Contains("user.png"))
                    {
                        var id = Avatar.Split(new string[] { $"/{type}/" }, StringSplitOptions.None)[1];
                        var size = 330;
                        if (!id.Contains("&mode=crop&scale=both") || !Avatar.Contains("cloudimage"))
                            return ImgHelper.Avatar(MaaSConfig.Current.Environment.AppUrl + $"cloudimage/{type}/" + id, size);
                        return Avatar;
                    }
                    if (Avatar.Contains("user.png"))
                        return "/assets/images/user.png";
                }
            }
            catch { }

            return "/assets/images/user.png";
        }
    }
}