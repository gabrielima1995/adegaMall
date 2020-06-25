using Application.Config;
using Application.Modules.Portfolio.SubCategories;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces;
using Domain.Modules.Portfolio;
using Domain.Services;
using Framework;
using Framework.Helpers;
using Infra.Crosscutting.Embed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.Modules.Portfolio
{
    /// <summary>
    /// View model do cadastro de um item do portfólio
    /// </summary>
    public class PortfolioItemViewModel : IWithCustomFields
    {
        private IList<PortfolioItemImage> _images;

        public Guid Id { get; set; }

        public string SellerId { get; set; }

        [DisplayNameLocalized("TITLE_PORTFOLIO_ITEM_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Title { get; set; }

        [DisplayNameLocalized("DESCRIPTION_PORTFOLIO_ITEM_LABEL")]
        [StringLengthLocalized(2024)]
        public string Description { get; set; }

        [DisplayNameLocalized("VIDEO_URL_PORTFOLIO_ITEM_LABEL")]
        public string VideoUrl { get; set; }

        [DisplayNameLocalized("PRICE_PORTFOLIO_ITEM_LABEL")]
        [RequiredIfLocalized("PriceRequired")]
        public decimal? Price { get; set; }

        public bool PriceRequired => MaaSConfig.Current.Portfolio.Price.IsRequired;

        [DisplayNameLocalized("NEW_IMAGES_PORTFOLIO_ITEM_LABEL")]
        public IList<PortfolioItemImage> Images
        {
            get { return _images ?? (_images = new List<PortfolioItemImage>()); }
            set { _images = value; }
        }

        public IList<SubCategoryAndCategorySelector> SubCategories { get; set; } = new List<SubCategoryAndCategorySelector>();

        public string CustomThumbnailUrl { get; set; }

        public bool IsVideo { get; set; }

        public string FriendlyUrl { get; set; }

        public string PublicPageUrl => $"{MaaSConfig.Current.Environment.AppUrl}portfolio/item/{FriendlyUrl}";

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Portfolio;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        public PortfolioItemViewModel()
        {
        }

        public PortfolioItemViewModel(PortfolioItem portfolioItem)
        {
            Id = portfolioItem.Id;
            Title = portfolioItem.Title;
            Description = portfolioItem.Description;
            VideoUrl = portfolioItem.VideoUrl;
            Price = portfolioItem.Price;
            Images = portfolioItem.Images ?? new List<PortfolioItemImage>(0);
            IsVideo = portfolioItem.IsVideo;
            FriendlyUrl = portfolioItem.FriendlyUrl;
            SubCategories = portfolioItem.SubCategories.Select(x => new SubCategoryAndCategorySelector
            {
                Category = x.Category_Id,
                SubCategory = x.Id
            }).ToList();
        }

        public PortfolioItemViewModel(string sellerId)
        {
            SellerId = sellerId;
        }

        private void SetCoverImageUrl(PortfolioItem portfolioItem, ICloudStorage cloudStorage)
        {
            if (portfolioItem.IsVideo)
            {
                try
                {
                    if (!string.IsNullOrEmpty(CustomThumbnailUrl))
                    {
                        portfolioItem.CoverImageUrl = CustomThumbnailUrl;
                    }
                    else if (string.IsNullOrEmpty(portfolioItem.CoverImageUrl))
                    {
                        var thumbUrl = portfolioItem.VideoUrl.GetThumbnail();
                        var fileName = Path.GetFileName(thumbUrl);
                        var extension = Path.GetExtension(thumbUrl);
                        if (extension.Length > 10 && extension.Contains(".jpg"))
                            fileName = fileName.Replace(extension, ".jpg");

                        if (thumbUrl.StartsWith("/assets/images/"))
                            thumbUrl = MaaSConfig.Current.Environment.AppUrl + thumbUrl;

                        var uri = cloudStorage.UploadFileFromUri(CloudFileType.PortfolioImage.ToString(), fileName, new Uri(thumbUrl));
                        portfolioItem.CoverImageUrl = uri.ToString();
                    }
                }
                catch (Exception) { }
            }
            else
            {
                portfolioItem.CoverImageUrl = portfolioItem.Images?.FirstOrDefault()?.Url;
            }
        }

        public PortfolioItem Create(Domain.Modules.Portfolio.Portfolio portfolio, IList<SubCategory> subCategories,
            ICloudStorage cloudStorage, FriendlyUrlGenerator friendlyUrlGenerator)
        {
            var entity = new PortfolioItem
            {
                Title = Title,
                Description = Description,
                VideoUrl = VideoUrl,
                Price = Price,
                Images = Images.FixPosition(),
                Portfolio = portfolio,
                SubCategories = subCategories
            };

            SetCoverImageUrl(entity, cloudStorage);
            friendlyUrlGenerator.Generate(entity);

            return entity;
        }

        public void Update(PortfolioItem entity, IList<SubCategory> subCategories,
            ICloudStorage cloudStorage, FriendlyUrlGenerator friendlyUrlGenerator)
        {
            entity.Title = Title;
            entity.Description = Description;
            entity.VideoUrl = IsVideo ? VideoUrl : null;
            entity.Price = Price;

            Images = Images.FixPosition();

            var imagesToRemove = entity.Images.Where(i => IsVideo || Images.All(j => i.Url != j.Url)).ToList();

            foreach (var image in imagesToRemove)
                entity.Images.Remove(image);

            var imagesToAdd = Images.Where(i => !IsVideo && entity.Images.All(j => i.Url != j.Url)).ToList();

            foreach (var image in imagesToAdd)
                entity.Images.Add(image);

            entity.SubCategories.Clear();

            foreach (var subCategory in subCategories)
                entity.SubCategories.Add(subCategory);

            entity.Images.FixPosition(true);

            SetCoverImageUrl(entity, cloudStorage);

            entity.FriendlyUrl = FriendlyUrl;
            if (string.IsNullOrEmpty(entity.FriendlyUrl)) friendlyUrlGenerator.Generate(entity);
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (IsVideo)
            {
                if (string.IsNullOrWhiteSpace(VideoUrl))
                {
                    errors.Add(nameof(VideoUrl), CultureInfo.CurrentCulture.GetStringLocalized("PORTFOLIO_ITEM_VIDEO_URL_REQUIRED"));
                }
                else
                {
                    var success = true;
                    var resource = ResourceEmbedFactory.GetResource(VideoUrl);
                    //success = resource.GetType() != typeof(UnknownEmbed);

                    if (!success)
                        errors.Add(nameof(VideoUrl), CultureInfo.CurrentCulture.GetStringLocalized("PORTFOLIO_ITEM_VIDEO_URL_INVALID"));
                }
            }

            if (!IsVideo && Images.Count == 0 && (!Images.Any() || Images.FirstOrDefault() == null))
            {
                errors.Add(nameof(Images), CultureInfo.CurrentCulture.GetStringLocalized("PORTFOLIO_ITEM_IMAGES_REQUIRED"));
            }

            return errors;
        }
    }
}