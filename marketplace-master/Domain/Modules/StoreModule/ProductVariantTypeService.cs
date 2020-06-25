using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.Products.Variants
{
    public class ProductVariantTypeService : ServiceBase<ProductVariantType>
    {
        public ProductVariantTypeService(
            IRepository<ProductVariantType> repository)
            : base(repository)
        {
        }

        public IList<ProductVariantType> SelectorList()
        {
            return Repository.Query()
                .Where(i => !i.Deleted)
                .OrderBy(i => i.Order)
                .ToList();
        }

        public IReadOnlyCollection<ProductVariantTypeAndItemDto> ListVariantTypesAndItems()
        {
            var types = Repository.Query(type => type.Variants)
                .Where(type => !type.Deleted)
                .OrderBy(type => type.Order)
                .ToList();

            var list = new List<ProductVariantTypeAndItemDto>();
            foreach (var type in types)
            {
                var items = type.Variants.Where(item => !item.Deleted).OrderBy(item => item.Order).ToList();
                list.AddRange(items.Select(item => new ProductVariantTypeAndItemDto
                {
                    TypeId = type.Id,
                    TypeName = type.Name,
                    ItemId = item.Id,
                    ItemName = item.Name
                }));
            }

            return list.AsReadOnly();
        }
    }

    public class ProductVariantTypeAndItemDto
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string FullName => ToString();

        public override string ToString() =>
            $"{TypeName} ({TypeId}) / {ItemName} ({ItemId})";
    }
}
