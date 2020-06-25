using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IProductCategoryRepository : ICrudRepository<ProductCategory>
    {
        List<ProductCategory> ListActive();
    }
}
