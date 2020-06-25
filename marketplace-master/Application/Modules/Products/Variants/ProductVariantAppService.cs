using Domain.Entities.StoreModule;
using Domain.Modules.Products.Variants;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.Products.Variants
{
    public class ProductVariantAppService : IProductVariantAppService
    {
        private readonly ProductVariantService _variantService;
        private readonly ProductVariantTypeService _variantTypeService;

        public ProductVariantAppService(
            ProductVariantService variantService,
            ProductVariantTypeService variantTypeService)
        {
            _variantService = variantService;
            _variantTypeService = variantTypeService;
        }

        public IList<ProductVariantListItemViewModel> GetList()
        {
            var list = _variantService
               .Query(i => i.Type)
               .Where(i => !i.Deleted && !i.Type.Deleted)
               .OrderBy(i => i.Type.Order)
               .ThenBy(i => i.Name)
               .ToList()
               .Select(i => new ProductVariantListItemViewModel(i))
               .ToList();

            return list;
        }

        public ProductVariantViewModel PrepareNewProductVariant()
        {
            var variantTypes = _variantTypeService.SelectorList();

            return new ProductVariantViewModel(variantTypes);
        }

        public ProductVariantViewModel PrepareProductVariant(ProductVariantViewModel model)
        {
            model.VariantTypes = _variantTypeService.SelectorList();

            return model;
        }

        public ProductVariantViewModel GetVariant(int id)
        {
            var variant = _variantService.Get(i =>
                i.Id == id && !i.Deleted,
                i => i.Type);

            var variantTypes = _variantTypeService.SelectorList();

            return variant == null
                ? null
                : new ProductVariantViewModel(variant, variantTypes);
        }

        public OperationResult<ProductVariant> SaveProductVariant(ProductVariantViewModel model)
        {
            var variantType = _variantTypeService.Get(i => i.Id == model.VariantType && !i.Deleted);

            return _variantService.Save(
                i => i.Id == model.Id && !i.Deleted,
                () => model.Create(variantType),
                entity => model.Update(entity, variantType));
        }

        public void DeleteProductVariant(int id)
        {
            _variantService.Update(
               i => i.Id == id && !i.Deleted,
               entity => { entity.Deleted = true; },
               i => i.Type);
        }

        public ProductVariantsSelector GetSelector(int? variantType)
        {
            var response = new ProductVariantsSelector
            {
                ProductVariants = _variantService.SelectorList(variantType)
            };

            return response;
        }
    }
}