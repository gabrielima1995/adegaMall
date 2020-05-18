using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithProductVariant
    {
        IList<ProductVariant> Variants { get; }

        int? Variant { get; }
    }
}