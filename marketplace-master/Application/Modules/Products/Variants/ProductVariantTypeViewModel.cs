using Domain.Entities.StoreModule;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantTypeViewModel
    {
        public int Id { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(50)]
        public string Name { get; set; }

        [DisplayNameLocalized("PRODUCT_VARIANT_TYPE_ORDER_LABEL")]
        [RequiredLocalized]
        public int Order { get; set; }

        public ProductVariantTypeViewModel()
        {
        }

        public ProductVariantTypeViewModel(ProductVariantType model)
        {
            Id = model.Id;
            Name = model.Name;
            Order = model.Order;
        }

        public ProductVariantType Create()
        {
            var entity = new ProductVariantType();

            entity.Name = Name;
            entity.Order = Order;

            return entity;
        }

        public void Update(ProductVariantType variantType)
        {
            variantType.Name = Name;
            variantType.Order = Order;
        }
    }
}
