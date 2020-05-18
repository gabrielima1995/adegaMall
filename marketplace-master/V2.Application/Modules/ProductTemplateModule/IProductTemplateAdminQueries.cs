using System;
using System.Collections.ObjectModel;

namespace V2.Application.Modules.ProductTemplateModule
{
    public interface IProductTemplateAdminQueries
    {
        ReadOnlyCollection<ProductTemplateAdminListItemDto> ListProductTemplateItems();
    }

    public class ProductTemplateAdminListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public ReadOnlyCollection<string> Variants { get; set; }
        public string CoverImageUrl { get; set; }
        public bool IsEnabled { get; set; }
        public decimal? DimensionsLength { get; set; }
        public decimal? DimensionsWidth { get; set; }
        public decimal? DimensionsHeight { get; set; }
        public decimal? DimensionsWeight { get; set; }
        public int ProductsCount { get; set; }
        public decimal GrossRevenue { get; set; }
    }
}
