using Application.Config;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class ProductEditing
    {
        public Guid Id { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Title { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(8000)]
        public string Description { get; set; }

        [RequiredIfLocalized("SpecificationsRequired")]
        [StringLengthLocalized(8000)]
        public string Specifications { get; set; }

        public bool SpecificationsRequired => MaaSConfig.Current.Product.Specifications.IsRequired;

        [RequiredIfLocalized("PriceRequired")]
        [RangeLocalized(0D, 999999999D)]
        public decimal? Price { get; set; }

        public bool PriceRequired => MaaSConfig.Current.Product.Price.IsRequired;

        [RangeLocalized(0D, 999999999D)]
        public decimal CurrentStock { get; set; }

        [StringLengthLocalized(100)]
        public string CustomId { get; set; }
    }
}