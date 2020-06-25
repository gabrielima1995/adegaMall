using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoryViewModel : IWithCategory
    {
        public int Id { get; set; }

        [DisplayNameLocalized("SUB_CATEGORY_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        [DisplayNameLocalized("CATEGORY_LABEL")]
        [RequiredLocalized]
        public int? Category { get; set; }

        [DisplayNameLocalized("CATEGORY_LABEL")]
        public IList<Category> Categories { get; set; }

        public SubCategoryViewModel()
        {
        }

        public SubCategoryViewModel(IList<Category> categories)
            : this()
        {
            Categories = categories;
        }

        public SubCategoryViewModel(SubCategory subCategory, IList<Category> categories)
            : this(categories)
        {
            Id = subCategory.Id;
            Name = subCategory.Name;
            Category = subCategory.Category.Id;
        }

        public SubCategory Create(Category category)
        {
            var entity = new SubCategory();

            entity.Name = Name;
            entity.Category = category;

            return entity;
        }

        public void Update(SubCategory subCategory, Category category)
        {
            subCategory.Name = Name;
            subCategory.Category = category;
        }
    }
}