using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithProductVariantType
    {
        IList<ProductVariantType> VariantTypes { get; }

        int? VariantType { get; }
    }
}