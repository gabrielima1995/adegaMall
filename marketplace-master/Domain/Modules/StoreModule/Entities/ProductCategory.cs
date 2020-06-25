using System;
using System.Collections.Generic;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Categoria de produto
    /// </summary>
    public class ProductCategory : SoftDeletableEntity<int>
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

        public IList<Product> Products { get; set; }

        public ProductCategory()
        {
        }

        public ProductCategory(string name)
        {
            Name = name;
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