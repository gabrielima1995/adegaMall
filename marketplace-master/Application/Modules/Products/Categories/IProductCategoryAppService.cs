using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;

namespace Application.Modules.Products.Categories
{
    public interface IProductCategoryAppService
    {
        IList<ProductCategoryViewModel> GetList();

        ProductCategoryViewModel PrepareNewCategory();

        ProductCategoryViewModel GetCategory(int id);

        OperationResult<ProductCategory> SaveCategory(ProductCategoryViewModel model);

        void DeleteCategory(int id);
    }
}