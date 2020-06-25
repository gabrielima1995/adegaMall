using System.Collections.Generic;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Categoria de produto
    /// </summary>
    public class ProductCategory : SoftDeletableEntity<int>
    {
        public string Name { get; set; }

        public IList<Product> Products { get; set; }

        public ProductCategory() { }

        public ProductCategory(string name)
        {
            Name = name;
        }
    }
}