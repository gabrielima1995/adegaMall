using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Application.Modules.Products
{
    public class CategoryMenuItem
    {
        public ProductVariantType VariantType { get; private set; }

        public string Title { get; private set; }

        public List<ProductVariant> Variants { get; private set; }

        public CategoryMenuItem(ProductVariantType variantType,
            string title,
            List<ProductVariant> variants)
        {
            VariantType = variantType;
            Title = title;
            Variants = variants;
        }
    }
}