using System;

namespace Presentation.Areas.Admin.Models
{
    public class CustomProductViewModel
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string TargetUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CategoryId { get; set; }
    }
}