using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;

namespace Application.Modules.Products.Variants
{
    public interface IProductVariantTypeAppService
    {
        IList<ProductVariantTypeViewModel> GetList();

        void DeleteVariantType(int id);

        ProductVariantTypeViewModel GetVariantType(int id);

        ProductVariantTypeViewModel PrepareNewVariantType();

        OperationResult<ProductVariantType> SaveVariantType(ProductVariantTypeViewModel model);
    }
}