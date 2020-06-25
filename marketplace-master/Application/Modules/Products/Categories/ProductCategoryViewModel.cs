using Domain.Entities.StoreModule;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Categories
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }

        [DisplayNameLocalized("PRODUCT_CATEGORY_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        public ProductCategoryViewModel()
        {
        }

        public ProductCategoryViewModel(ProductCategory category)
            : this()
        {
            Id = category.Id;
            Name = category.Name;
        }

        public ProductCategory Create()
        {
            var entity = new ProductCategory();

            entity.Name = Name;

            return entity;
        }

        public void Update(ProductCategory category)
        {
            category.Name = Name;
        }

        public override string ToString()
        {
            return $"{Id}, {Name}";
        }
    }
}