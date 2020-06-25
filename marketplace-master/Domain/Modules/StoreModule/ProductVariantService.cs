using Domain.Entities.StoreModule;
using Framework;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.Products.Variants
{
    public class ProductVariantService : ServiceBase<ProductVariant>
    {
        public ProductVariantService(IRepository<ProductVariant> repository)
            : base(repository)
        {
        }

        public IList<ProductVariant> SelectorList(int? typeId = null)
        {
            return Repository.Query()
                .Where(i => !i.Deleted)
                .Where(i => !i.Type.Deleted)
                .Where(i => typeId == null || i.Type.Id == typeId)
                .OrderBy(i => i.Type.Order)
                .ThenBy(i => i.Type.Name)
                .ThenBy(i => i.Order)
                .ThenBy(i => i.Name)
                .ToList();
        }
    }
}
