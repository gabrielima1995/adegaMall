using Domain.Modules.StoreModule.Price;
using System;
using System.Collections.ObjectModel;

namespace V2.Application.Modules.ProductTemplateModule
{
    public interface IProductTemplateStoreQueries
    {
        ReadOnlyCollection<StoreTemplateItemDto> ListStoreTemplates(string sellerId, int startIndex, int pageSize);
    }

    public class StoreTemplateItemDto
    {
        public Guid TemplateId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }

        public Guid? SellerProductId { get; set; }
        public ProductPrice Price { get; set; }
        public int? CurrentStock { get; set; }
    }
}
