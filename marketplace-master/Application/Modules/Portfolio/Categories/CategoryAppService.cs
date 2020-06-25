using Domain.Categories;
using Domain.Entities.PortfolioModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Portfolio.Categories
{
    public class CategoryAppService : ICategoryAppService
    {
        private readonly CategoryService_Obsolete _categoryService;

        public CategoryAppService(
            CategoryService_Obsolete categoryService)
        {
            _categoryService = categoryService;
        }

        public IList<CategoryViewModel> GetList()
        {
            var list = _categoryService
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList()
                .Select(i => new CategoryViewModel(i))
                .ToList();

            return list;
        }

        public CategoryViewModel PrepareNewCategory()
        {
            return new CategoryViewModel();
        }

        public CategoryViewModel GetCategory(int id)
        {
            var category = _categoryService
                .Get(i => i.Id == id && !i.Deleted);

            return category == null
                ? null
                : new CategoryViewModel(category);
        }

        public OperationResult<Category> SaveCategory(CategoryViewModel model)
        {
            return _categoryService.Save(
                i => i.Id == model.Id && !i.Deleted,
                model.Create,
                model.Update);
        }

        public void DeleteCategory(int id)
        {
            _categoryService.Update(
               i => i.Id == id && !i.Deleted,
               entity =>
               {
                   entity.Deleted = true;
               });
        }
    }
}