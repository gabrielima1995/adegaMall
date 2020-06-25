using Domain.Entities.PortfolioModule;
using Framework;
using System.Collections.Generic;

namespace Application.Modules.Portfolio.Categories
{
    public interface ICategoryAppService
    {
        IList<CategoryViewModel> GetList();

        CategoryViewModel PrepareNewCategory();

        CategoryViewModel GetCategory(int id);

        OperationResult<Category> SaveCategory(CategoryViewModel model);

        void DeleteCategory(int id);
    }
}