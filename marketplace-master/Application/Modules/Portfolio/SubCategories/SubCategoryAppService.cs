using Domain.Categories;
using Domain.Entities.PortfolioModule;
using Domain.SubCategories;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Portfolio.SubCategories
{
    public class SubCategoryAppService : ISubCategoryAppService
    {
        private readonly SubCategoryService_Obsolete _subCategoryService;
        private readonly CategoryService_Obsolete _categoryService;

        public SubCategoryAppService(
            SubCategoryService_Obsolete subCategoryService,
            CategoryService_Obsolete categoryService)
        {
            _subCategoryService = subCategoryService;
            _categoryService = categoryService;
        }

        public IList<SubCategoryListItemViewModel> GetList()
        {
            var list = _subCategoryService
               .Query(i => i.Category)
               .Where(i => !i.Deleted)
               .OrderBy(i => i.Name)
               .ToList()
               .Select(i => new SubCategoryListItemViewModel(i))
               .ToList();

            return list;
        }

        public SubCategoryViewModel PrepareNewSubCategory()
        {
            var categories = _categoryService.SelectorList();

            return new SubCategoryViewModel(categories);
        }

        public SubCategoryViewModel PrepareSubCategory(SubCategoryViewModel model)
        {
            model.Categories = _categoryService.SelectorList();

            return model;
        }

        public SubCategoryViewModel GetSubCategory(int id)
        {
            var subCategory = _subCategoryService
                .Get(i => i.Id == id && !i.Deleted,
                    i => i.Category);
            var categories = _categoryService.SelectorList();

            return subCategory == null
                ? null
                : new SubCategoryViewModel(subCategory, categories);
        }

        public OperationResult<SubCategory> SaveSubCategory(SubCategoryViewModel model)
        {
            var category = _categoryService.Get(i => i.Id == model.Category && !i.Deleted);

            return _subCategoryService.Save(
                i => i.Id == model.Id && !i.Deleted,
                () => model.Create(category),
                entity => model.Update(entity, category));
        }

        public void DeleteSubCategory(int id)
        {
            _subCategoryService.Update(
               i => i.Id == id && !i.Deleted,
               entity =>
               {
                   entity.Deleted = true;
               });
        }

        public SubCategoriesSelector GetSelector(int? category, string sellerId)
        {
            var response = new SubCategoriesSelector
            {
                SubCategories = _subCategoryService.SelectorList(category, sellerId)
            };

            return response;
        }
    }
}