using Domain.Entities;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantTypeAndItemSelector : IWithProductVariantType, IWithProductVariant
    {
        public bool VariantTypeNotNull => VariantType.HasValue;
        public bool VariantNotNull => Variant.HasValue;

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_LABEL")]
        [RequiredIfLocalized("VariantNotNull")]
        public int? VariantType { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_LABEL")]
        [RequiredIfLocalized("VariantTypeNotNull")]
        public int? Variant { get; set; }

        public IList<ProductVariantType> VariantTypes { get; set; }

        public IList<ProductVariant> Variants { get; set; }
    }
}