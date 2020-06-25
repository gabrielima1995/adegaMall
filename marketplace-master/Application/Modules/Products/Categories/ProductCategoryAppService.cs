using Domain.Entities.StoreModule;
using Domain.Services.StoreModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products.Categories
{
    public class ProductCategoryAppService : IProductCategoryAppService
    {
        private readonly ProductCategoryService _productCategoryService;

        public ProductCategoryAppService(ProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public IList<ProductCategoryViewModel> GetList()
        {
            var list = _productCategoryService
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList()
                .Select(i => new ProductCategoryViewModel(i))
                .ToList();

            return list;
        }

        public ProductCategoryViewModel PrepareNewCategory()
        {
            return new ProductCategoryViewModel();
        }

        public ProductCategoryViewModel GetCategory(int id)
        {
            var category = _productCategoryService
                .Get(i => i.Id == id && !i.Deleted);

            return category == null
                ? null
                : new ProductCategoryViewModel(category);
        }

        public OperationResult<ProductCategory> SaveCategory(ProductCategoryViewModel model)
        {
            return _productCategoryService.Save(
                i => i.Id == model.Id && !i.Deleted,
                model.Create,
                model.Update);
        }

        public void DeleteCategory(int id)
        {
            _productCategoryService.Update(
                i => i.Id == id && !i.Deleted,
                entity =>
                {
                    entity.Deleted = true;
                });
        }
    }
}