using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Application.Modules.Products.Variants
{
    public class ProductStockVariantItemViewModel
    {
        public int Id { get; set; }

        public bool StockVariantNotNull => StockVariant.HasValue;

        [DisplayNameLocalized("PRODUCT_STOCK_VARIANT_LABEL")]
        [RequiredIfLocalized("StockVariantNotNull")]
        public int? StockVariant { get; set; }

        public IList<ProductVariant> StockVariants { get; set; } = new List<ProductVariant>();

        [DisplayNameLocalized("PRODUCT_STOCK_VARIANT_STOCK_CURRENT_LABEL")]
        public int? CurrentStock { get; set; }

        [DisplayNameLocalized("PRODUCT_STOCK_VARIANT_STOCK_UPDATED_LABEL")]
        public int? UpdatedStock { get; set; }

        public ProductStockVariantItemViewModel()
        {
        }

        public ProductStockVariantItemViewModel(StockProductVariant item)
        {
            Id = item.Id;
            StockVariant = item.Variant.Id;
            CurrentStock = item.CurrentStock;
            UpdatedStock = item.CurrentStock;
        }

        public StockProductVariant Create(IList<ProductVariant> variants)
        {
            var variant = variants.FirstOrDefault(i => i.Id == StockVariant);

            return StockProductVariant.New(variant, CurrentStock ?? 0, UpdatedStock ?? (CurrentStock ?? 0));
        }
    }
}