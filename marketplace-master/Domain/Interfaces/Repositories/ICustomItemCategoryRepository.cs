using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface ICustomItemCategoryRepository : ICrudRepository<CustomItemCategory>
    {
        List<CustomItemCategory> ListAll();
    }
}
