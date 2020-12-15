using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Areas.Admin.Models
{
    public class BannerIndexVM
    {
        public List<BannerVM> Items { get; set; }

        public BannerIndexVM()
        {
            Items = new List<BannerVM>();
        }
    }

    public class BannerCreateEditVM
    {
        public BannerVM Banner { get; set; }
    }

    public class BannerDeleteViewModel
    {
        public BannerVM Banner { get; set; }
    }

    public class BannerVM
    {
        public Guid Id { get; set; }

        [DisplayNameLocalized("BANNER_IMAGE_URL_LABEL")]
        public string ImageUrl { get; set; }

        [DisplayNameLocalized("BANNER_MOBILE_IMAGE_URL_LABEL")]
        public string MobileImageUrl { get; set; }

        [Url]
        [DisplayNameLocalized("BANNER_ACTION_URL_LABEL")]
        [RequiredLocalized]
        public string ActionUrl { get; set; }

        [DataType(DataType.Date)]
        [DisplayNameLocalized("BANNER_STARTS_AT_LABEL")]
        public DateTime? StartsAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayNameLocalized("BANNER_EXPIRES_AT_LABEL")]
        public DateTime? ExpiresAt { get; set; }

        public string ImageThumbSm => ImgHelper.BlobToCdn(ImageUrl, new { w = 100, h = 27, scale = "both", mode = "crop" });

        public string ImageThumbLg => ImgHelper.BlobToCdn(ImageUrl, new { w = 300, h = 82, scale = "both", mode = "crop" });

        public string MobileImageThumbLg => ImgHelper.BlobToCdn(MobileImageUrl, new { w = 300, h = 212, scale = "both", mode = "crop" });

        [DisplayName("Ordem")]
        public int Order { get; set; }

        public BannerVM()
        {
        }

        public BannerVM(Banner banner)
        {
            Mapper.Map(banner, this);
        }

        public Banner ToDomainModel(Banner banner)
        {
            return Mapper.Map(this, banner);
        }
    }
}