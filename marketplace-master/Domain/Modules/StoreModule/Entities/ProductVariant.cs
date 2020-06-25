using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.StoreModule
{
    public class ProductVariant : SoftDeletableEntity<int>
    {
        public string Name { get; set; }

        [Required]
        public ProductVariantType Type { get; set; }

        public IList<Product> AssociatedProducts { get; set; }

        public int Order { get; set; }

        public IList<StockProductVariant> StockVariants { get; set; }

        public ProductVariant()
        {
            AssociatedProducts = new List<Product>();
        }
    }
}