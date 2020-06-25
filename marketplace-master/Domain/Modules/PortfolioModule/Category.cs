using System.Collections.Generic;

namespace Domain.Entities.PortfolioModule
{
    /// <summary>
    /// Categoria de portfólio
    /// </summary>
    public class Category : SoftDeletableEntity<int>
    {
        public string Name { get; set; }

        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public Category() { }

        public Category(string name)
        {
            Name = name;
        }

        public void AddSubcategory(SubCategory subCategory)
        {
            SubCategories.Add(subCategory);
            subCategory.Category = this;
        }
    }
}