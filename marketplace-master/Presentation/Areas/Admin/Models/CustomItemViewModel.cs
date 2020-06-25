using Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Presentation.Areas.Admin.Models
{
    public class CustomItemViewModel
    {
        public bool IsEditing { get; set; }
        public HttpPostedFileBase UploadedImage { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string TargetUrl { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? Id { get; set; }

        public CustomItem ToEntity()
        {
            CustomItem entity = new CustomItem()
            {
                CategoryId = this.CategoryId,
                EndDate = this.EndDate,
                ImageUrl = this.ImageUrl,
                TargetUrl = this.TargetUrl,
                StartDate = this.StartDate,
                Title = this.Title,
                Id = this.Id ?? Guid.NewGuid(),
                CreatedAt = CreatedAt ?? DateTime.Now,
            };

            if (EndDate.GetValueOrDefault().Year < 1900)
                entity.EndDate = null;

            if (StartDate.GetValueOrDefault().Year < 1900)
                entity.StartDate = null;

            return entity;
        }

        public CustomItemViewModel(CustomItem item)
        {
            Id = item.Id;
            Title = item.Title;
            ImageUrl = item.ImageUrl;
            TargetUrl = item.TargetUrl;
            StartDate = item.StartDate;
            EndDate = item.EndDate;
            CategoryId = item.CategoryId;
            CreatedAt = item.CreatedAt;
        }

        public CustomItemViewModel()
        {

        }

        public string GetDateString()
        {
            string date1 = StartDate?.ToShortDateString() ?? "";
            string date2 = EndDate?.ToShortDateString() ?? "";
            return $"{date1} - {date2}";
        }
    }
}