using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface ICustomItemRepository : ICrudRepository<CustomItem>
    {
        List<CustomItem> ListAll();
    }
}
