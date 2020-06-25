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

        [Url]
        [DisplayNameLocalized("BANNER_IMAGE_URL_LABEL")]
        public string ImageUrl { get; set; }

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

        public string ImageThumbSm => ImgHelper.BlobToCdn(ImageUrl, new { w = 100, h = 32, scale = "both", mode = "crop" });

        public string ImageThumbLg => ImgHelper.BlobToCdn(ImageUrl, new { w = 300, h = 94, scale = "both", mode = "crop" });

        public BannerVM()
        {
        }

        public BannerVM(Banner banner)
        {
            Mapper.Map(banner, this);
        }

        public Banner ToDomainModel()
        {
            return Mapper.Map<Banner>(this);
        }
    }
}