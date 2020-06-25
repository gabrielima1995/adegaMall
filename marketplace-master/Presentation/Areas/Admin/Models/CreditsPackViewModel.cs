using Application.Config;
using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Areas.Admin.Models
{
    public class CreditsPackViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Name_EN { get; set; }

        public string Description { get; set; }

        public string Description_EN { get; set; }

        [Required]
        public int CreditsAmount { get; set; }

        [Required]
        public decimal Price { get; set; }

        [RequiredIfLocalized("ValidityRequired")]
        public int? ValidityDays { get; set; }

        public bool ValidityRequired => MaaSConfig.Current.Quote.LeadSales.CreditsExpire;

        public CreditsPackViewModel()
        {
            if (ValidityRequired)
                ValidityDays = MaaSConfig.Current.Quote.LeadSales.CreditsDefaultDuration;
        }
    }
}