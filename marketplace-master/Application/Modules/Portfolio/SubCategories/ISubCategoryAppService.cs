using Domain.Entities.PortfolioModule;
using Framework;
using System.Collections.Generic;

namespace Application.Modules.Portfolio.SubCategories
{
    public interface ISubCategoryAppService
    {
        IList<SubCategoryListItemViewModel> GetList();

        SubCategoryViewModel PrepareNewSubCategory();

        SubCategoryViewModel PrepareSubCategory(SubCategoryViewModel model);

        SubCategoryViewModel GetSubCategory(int id);

        OperationResult<SubCategory> SaveSubCategory(SubCategoryViewModel model);

        void DeleteSubCategory(int id);

        SubCategoriesSelector GetSelector(int? category, string sellerId);
    }
}