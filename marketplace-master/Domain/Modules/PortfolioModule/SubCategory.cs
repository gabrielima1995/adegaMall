using Domain.Modules.Portfolio;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.PortfolioModule
{
    /// <summary>
    /// Sub-categoria de portfólio
    /// </summary>
    public class SubCategory : SoftDeletableEntity<int>
    {
        public string Name { get; set; }

        public int? Category_Id { get; set; }

        public IList<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

        [ForeignKey("Category_Id")]
        public Category Category { get; set; }

        public SubCategory() { }

        public SubCategory(string name)
        {
            Name = name;
        }
    }
}