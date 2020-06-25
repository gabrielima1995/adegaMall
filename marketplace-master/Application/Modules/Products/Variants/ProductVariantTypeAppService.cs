using Domain.Entities.StoreModule;
using Domain.Modules.Products.Variants;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantTypeAppService : IProductVariantTypeAppService
    {
        private readonly ProductVariantTypeService _productVariantTypeService;

        public ProductVariantTypeAppService(ProductVariantTypeService productVariantTypeService)
        {
            _productVariantTypeService = productVariantTypeService;
        }

        public IList<ProductVariantTypeViewModel> GetList()
        {
            var list = _productVariantTypeService
                .Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Order)
                .ToList()
                .Select(i => new ProductVariantTypeViewModel(i))
                .ToList();

            return list;
        }

        public ProductVariantTypeViewModel PrepareNewVariantType()
        {
            return new ProductVariantTypeViewModel();
        }

        public ProductVariantTypeViewModel GetVariantType(int id)
        {
            var variantType = _productVariantTypeService
                .Get(i => i.Id == id && !i.Deleted);

            return variantType == null
                ? null
                : new ProductVariantTypeViewModel(variantType);
        }

        public OperationResult<ProductVariantType> SaveVariantType(ProductVariantTypeViewModel model)
        {
            return _productVariantTypeService.Save(
                i => i.Id == model.Id && !i.Deleted,
                model.Create,
                model.Update);
        }

        public void DeleteVariantType(int id)
        {
            _productVariantTypeService.Update(
                i => i.Id == id && !i.Deleted,
                entity =>
                {
                    entity.Deleted = true;
                });
        }
    }
}
