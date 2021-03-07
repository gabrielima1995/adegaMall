using System;
using Application.Helpers;
using Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Presentation.Models
{
    public class BannerListVM
    {
        public List<BannerItemVM> Items { get; set; }

        public bool HasBanners => Items.Count > 0;

        public bool ShowNavControls => Items.Count > 1;

        public BannerListVM()
        {
            Items = new List<BannerItemVM>();
        }

        public BannerListVM(List<Banner> domainItems)
        {
            Items = domainItems.Select(x => new BannerItemVM(x)).ToList();
        }
    }

    [NotMapped]
    public class BannerItemVM
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string MobileImageUrl { get; set; }
        public string ActionUrl { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string OptimizedImgUrl =>
            ImgHelper.BlobToCdn(ImageUrl, new { width = 2500, height = 1000, mode = "crop", scale = "both" });

        public string OptimizedMobileImgUrl =>
            !string.IsNullOrEmpty(MobileImageUrl)
                ? ImgHelper.BlobToCdn(MobileImageUrl, new { width = 750, height = 600, mode = "crop", scale = "both" })
                : ImgHelper.BlobToCdn(ImageUrl, new { width = 1320, height = 530, mode = "crop", scale = "both" });

        public BannerItemVM()
        {
        }

        public BannerItemVM(Banner domainModel)
        {
            Id = domainModel.Id;
            StartsAt = domainModel.StartsAt;
            ExpiresAt = domainModel.ExpiresAt;
            ImageUrl = domainModel.ImageUrl;
            MobileImageUrl = domainModel.MobileImageUrl;
            ActionUrl = domainModel.ActionUrl;
        }
    }
}