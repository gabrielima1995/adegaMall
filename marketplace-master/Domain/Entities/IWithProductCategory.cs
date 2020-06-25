using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Domain.Entities
{
    public interface IWithProductCategory
    {
        IList<ProductCategory> Categories { get; }

        int? Category { get; }
    }
}