using System.Collections.Generic;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantComparer : IEqualityComparer<ProductVariantTypeAndItemSelector>
    {
        public bool Equals(ProductVariantTypeAndItemSelector x, ProductVariantTypeAndItemSelector y)
        {
            return x.VariantType == y.VariantType && x.Variant == y.Variant;
        }

        public int GetHashCode(ProductVariantTypeAndItemSelector obj)
        {
            return obj.Variant ?? 0 + obj.VariantType ?? 0;
        }
    }
}