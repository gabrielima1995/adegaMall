using Domain.Entities;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantViewModel : IWithProductVariantType
    {
        public int Id { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_NAME_LABEL")]
        public string Name { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_LABEL")]
        public int? VariantType { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_ORDER_LABEL")]
        [RequiredLocalized]
        public int Order { get; set; }

        public IList<ProductVariantType> VariantTypes { get; set; }

        public ProductVariantViewModel()
        {
        }

        public ProductVariantViewModel(IList<ProductVariantType> variantTypes)
            : this()
        {
            VariantTypes = variantTypes;
        }

        public ProductVariantViewModel(ProductVariant variant, IList<ProductVariantType> variantTypes)
            : this(variantTypes)
        {
            Id = variant.Id;
            Name = variant.Name;
            VariantType = variant.Type.Id;
            Order = variant.Order;
        }

        public ProductVariant Create(ProductVariantType variantType)
        {
            var entity = new ProductVariant();

            entity.Name = Name;
            entity.Type = variantType;
            entity.Order = Order;

            return entity;
        }

        public void Update(ProductVariant variant, ProductVariantType variantType)
        {
            variant.Name = Name;
            variant.Type = variantType;
            variant.Order = Order;
        }
    }
}
