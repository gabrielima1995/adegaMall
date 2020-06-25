using Domain.Entities.StoreModule;
using System.ComponentModel;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantListItemViewModel
    {
        public int Id { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_NAME_LABEL")]
        public string Name { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_LABEL")]
        public string VariantType { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_ORDER_LABEL")]
        public int Order { get; set; }

        public ProductVariantListItemViewModel(ProductVariant variant)
        {
            Id = variant.Id;
            Name = variant.Name;
            VariantType = variant.Type.Name;
            Order = variant.Order;
        }
    }
}