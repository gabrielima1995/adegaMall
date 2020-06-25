using Domain.Entities.PortfolioModule;
using System.ComponentModel;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoryListItemViewModel
    {
        public int Id { get; set; }

        [DisplayNameLocalized("SUB_CATEGORY_NAME_LABEL")]
        public string Name { get; set; }

        [DisplayNameLocalized("CATEGORY_LABEL")]
        public string Category { get; set; }

        public bool CategoryDeleted { get; set; }

        public SubCategoryListItemViewModel(SubCategory subCategory)
        {
            Id = subCategory.Id;
            Name = subCategory.Name;
            Category = subCategory.Category.Name;
            CategoryDeleted = subCategory?.Category?.Deleted ?? false;
        }
    }
}