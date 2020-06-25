using Domain.Entities.PortfolioModule;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Portfolio.Categories
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [DisplayNameLocalized("CATEGORY_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        public CategoryViewModel()
        {
        }

        public CategoryViewModel(Category category)
            : this()
        {
            Id = category.Id;
            Name = category.Name;
        }

        public Category Create()
        {
            var entity = new Category();

            entity.Name = Name;

            return entity;
        }

        public void Update(Category category)
        {
            category.Name = Name;
        }
    }
}