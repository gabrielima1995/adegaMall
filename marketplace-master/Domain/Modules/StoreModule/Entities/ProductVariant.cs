using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.StoreModule
{
    public class ProductVariant : SoftDeletableEntity<int>
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Slugify();
            }
        }

        public string Slug { get; private set; }

        [Required]
        public ProductVariantType Type { get; set; }

        public IList<Product> AssociatedProducts { get; set; }

        public int Order { get; set; }

        public IList<StockProductVariant> StockVariants { get; set; }

        public ProductVariant()
        {
            AssociatedProducts = new List<Product>();
        }

        public void Slugify()
        {
            if (!string.IsNullOrEmpty(_name) && string.IsNullOrEmpty(Slug))
            {
                Slug = _name.ReplaceDiacritics()
                    .Hyphenize();
            }
        }
    }
}