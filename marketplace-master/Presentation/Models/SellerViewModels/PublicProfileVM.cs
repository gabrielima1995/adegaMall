using Application.Config;
using Application.Modules.Portfolio;
using Application.Modules.Ratings.Models;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Modules.Portfolio;
using Domain.Modules.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Models
{
    public class PublicProfileVM : IWithCustomFields
    {
        public Seller Seller { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public PublicProfilePortfolioVM Portfolio { get; set; }

        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public List<UserRatingViewModel> LatestRatings { get; set; } = new List<UserRatingViewModel>();

        public Dictionary<Category, List<SubCategory>> Categories { get; set; } = new Dictionary<Category, List<SubCategory>>();

        public MapLinksVM Links
        {
            get
            {
                var addrConfig = MaaSConfig.Current.Seller.Address;
                var addressEnabled = addrConfig.Enabled;
                var showAddress = addrConfig.ShowMap || addrConfig.ShowText;

                if (!addressEnabled || !showAddress) return null;
                if (Seller.Address == null) return null;

                return new MapLinksVM(
                    Seller?.PublicProfileName, "",
                    Seller?.Address?.Location?.Latitude,
                    Seller?.Address?.Location?.Longitude);
            }
        }

        public bool ShowLinkForMoreProducts { get; set; }
        public string LinkForMoreProducts { get; set; }
        public bool HasCategories => Seller.SellerCompleteProfileMapped.PortfolioCategories.IsComplete;

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Seller;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public Guid Id { get { return new Guid(Seller.Id); } set { } }

        public PublicProfileVM()
        {
        }

        public PublicProfileVM(SellerPublicProfile publicProfileDto)
        {
            Seller = publicProfileDto.Seller;
            Products = publicProfileDto.Products;
            Portfolio = new PublicProfilePortfolioVM(publicProfileDto.Portfolio);

            if (Portfolio != null && Portfolio.SubCategories != null)
            {
                foreach (var subCategory in Portfolio.SubCategories)
                {
                    if (!Categories.ContainsKey(subCategory.Category))
                    {
                        Categories.Add(subCategory.Category, new List<SubCategory>());
                    }

                    Categories[subCategory.Category].Add(subCategory);
                }
            }
        }

        public VisualizationSettings Visualization => new VisualizationSettings(this);
        public class VisualizationSettings
        {
            private static MaaSConfig Config => MaaSConfig.Current;
            private readonly PublicProfileVM model;

            public VisualizationSettings(PublicProfileVM model)
                => this.model = model;

            public bool ShowAdditionalInfo1 => Config.Seller.AdditionalInfo1.ShowInPublicProfile && Config.Seller.AdditionalInfo1.Enabled && !string.IsNullOrEmpty(model.Seller.AdditionalInfo1);
            public bool ShowAdditionalInfo2 => Config.Seller.AdditionalInfo2.ShowInPublicProfile && Config.Seller.AdditionalInfo2.Enabled && !string.IsNullOrEmpty(model.Seller.AdditionalInfo2);
            public bool ShowBirthDate => Config.Seller.DateOfBirth.ShowInPublicProfile && Config.Seller.DateOfBirth.Enabled && model.Seller.DateOfBirth.HasValue;
            public bool ShowCategories => model.Portfolio?.SubCategories?.Count > 0;
            public bool ShowCustomFields => Config.CustomFields.Seller.Enabled && (model.CustomFieldDefinitions?.Any(field => field.Value.IsPublic) ?? false) && (model.CustomFields?.Any(field => !string.IsNullOrEmpty(field.Value)) ?? false);
            public bool ShowGenre => Config.Seller.Genre.ShowInPublicProfile && Config.Seller.Genre.Enabled && model.Seller.Genre != null;

            public bool ShowLeftSide => ShowAdditionalInfo1 || ShowAdditionalInfo2 || ShowBirthDate || ShowCustomFields || ShowGenre;
            public bool ShowRightSide => ShowCategories;
            public bool ShowSides => ShowLeftSide || ShowRightSide;

            public bool ShowScheduleTimeline =>
                Config.Environment.FullCalendar.HasPremiumLicense &&
                Config.Product.BusinessModel.Rental.Seller.Schedule.Configuration.Profile.Enabled;
        }
    }

    public class PublicProfilePortfolioVM
    {
        public PublicProfilePortfolioVM()
        {
        }

        public PublicProfilePortfolioVM(Portfolio portfolio)
        {
            if (portfolio != null)
            {
                SubCategories = portfolio.SubCategories ?? new List<SubCategory>();
                SocialMedia = portfolio.SocialMedia ?? new List<SellerSocialMedia>();
                Items = portfolio.Items?.Select(x => new PortfolioItemVM(x)).ToList() ?? new List<PortfolioItemVM>();
            }
        }

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public IList<SellerSocialMedia> SocialMedia { get; set; } = new List<SellerSocialMedia>();
        public IList<PortfolioItemVM> Items { get; set; } = new List<PortfolioItemVM>();
    }
}