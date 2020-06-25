using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoryAndCategorySelector : IWithCategory, IWithSubCategory
    {
        [DisplayNameLocalized("CATEGORY_LABEL")]
        [RequiredLocalized]
        public int? Category { get; set; }

        [DisplayNameLocalized("SUB_CATEGORIES_LABEL")]
        [RequiredLocalized]
        public int? SubCategory { get; set; }

        public IList<Category> Categories { get; set; } = new List<Category>();

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}