using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantsSelector
    {
        public IList<ProductVariant> ProductVariants { get; set; }

        public int? ProductVariant { get; set; }
    }
}