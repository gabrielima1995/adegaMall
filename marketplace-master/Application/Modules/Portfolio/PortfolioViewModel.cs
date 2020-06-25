using Application.Config;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Modules.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Portfolio
{
    public class PortfolioViewModel : PortfolioRequestViewModel
    {
        public IList<PortfolioSocialMediaViewModel> SocialMedia { get; set; }

        public IList<PortfolioItemVM> Items { get; set; }

        public PortfolioViewModel()
        {
            SocialMedia = new List<PortfolioSocialMediaViewModel>();
        }
    }

    public class PortfolioItemVM : IWithCustomFields
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private int? order = null;

        private IList<PortfolioItemImage> _images;

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public decimal? Price { get; set; }

        public string CoverImageUrl { get; set; }

        public string FriendlyUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public IList<PortfolioItemImage> Images
        {
            get { return _images = _images?.OrderBy(i => i.Position).ToList(); }
            set { _images = value; }
        }

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public bool IsVideo => !string.IsNullOrEmpty(VideoUrl);

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Portfolio;

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }

        public Dictionary<string, string> CustomFields { get; set; }

        public Guid Id { get; set; }

        public string SellerId { get; set; }
        public Seller Seller { get; private set; }

        public bool HasPrice => MaaSConfig.Current.Portfolio.Price.Enabled &&
                                Price.HasValue &&
                                Price > 0M;

        public string FullPriceText => $"{MaaSConfig.Current.DefaultCurrency} {Price.Value.DisplayDecimal(2)}";

        public int? Order => (order = order ?? Random.Next(1, 100));

        public PortfolioItemVM()
        {
        }

        public PortfolioItemVM(PortfolioItem item)
        {
            this.CoverImageUrl = item.CoverImageUrl;
            this.CreatedAt = item.CreatedAt;
            this.Description = item.Description;
            this.Id = item.Id;
            this.Images = item.Images;
            this.Price = item.Price;
            this.SubCategories = item.SubCategories;
            this.Title = item.Title;
            this.VideoUrl = item.VideoUrl;
            this.SellerId = item.Portfolio.SellerId;
            this.Seller = item.Portfolio.Seller;
            this.FriendlyUrl = item.FriendlyUrl ?? Id.ToString();
        }
    }
}