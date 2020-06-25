using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;

namespace Application.Modules.Products.Variants
{
    public interface IProductVariantAppService
    {
        void DeleteProductVariant(int id);

        IList<ProductVariantListItemViewModel> GetList();

        ProductVariantsSelector GetSelector(int? variantType);

        ProductVariantViewModel GetVariant(int id);

        ProductVariantViewModel PrepareNewProductVariant();

        ProductVariantViewModel PrepareProductVariant(ProductVariantViewModel model);

        OperationResult<ProductVariant> SaveProductVariant(ProductVariantViewModel model);
    }
}