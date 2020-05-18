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
    public class BannerItemVM : Banner
    {
        public string OptimizedImgUrl => ImgHelper.BlobToCdn(ImageUrl, new { width = 1920, height = 530, mode = "crop", scale = "both" });

        public BannerItemVM()
        {
        }

        public BannerItemVM(Banner domainModel)
        {
            Id = domainModel.Id;
            StartsAt = domainModel.StartsAt;
            ExpiresAt = domainModel.ExpiresAt;
            ImageUrl = domainModel.ImageUrl;
            ActionUrl = domainModel.ActionUrl;
        }
    }
}