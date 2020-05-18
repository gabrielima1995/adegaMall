using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using System.Collections.Generic;

namespace V2.Infrastructure.Modules.Customization.Repositories
{
    public class CustomItemRepository : EfCrudRepository<CustomItem>, ICustomItemRepository
    {
        public CustomItemRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<CustomItem> ListAll()
        {
            return List();
        }
    }
}