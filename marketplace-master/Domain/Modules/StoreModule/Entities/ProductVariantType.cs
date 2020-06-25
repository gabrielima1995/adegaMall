using System.Collections.Generic;

namespace Domain.Entities.StoreModule
{
    public class ProductVariantType : SoftDeletableEntity<int>
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public IList<ProductVariant> Variants { get; set; }

        public IList<Product> AssociatedStockProducts { get; set; }

        public ProductVariantType()
        {
            Variants = new List<ProductVariant>();
        }
    }
}
