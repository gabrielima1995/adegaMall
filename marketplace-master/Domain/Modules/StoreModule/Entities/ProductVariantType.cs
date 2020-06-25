using System;
using System.Collections.Generic;

namespace Domain.Entities.StoreModule
{
    public class ProductVariantType : SoftDeletableEntity<int>
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

        public int Order { get; set; }

        public IList<ProductVariant> Variants { get; set; }

        public IList<Product> AssociatedStockProducts { get; set; }

        public ProductVariantType()
        {
            Variants = new List<ProductVariant>();
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
