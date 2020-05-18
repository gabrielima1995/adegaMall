using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using System.Collections.Generic;

namespace V2.Infrastructure.Modules.Customization.Repositories
{
    public class CustomItemCategoryRepository : EfCrudRepository<CustomItemCategory>, ICustomItemCategoryRepository
    {
        public CustomItemCategoryRepository(ScaffoldContext context) : base(context)
        {
        }

        public List<CustomItemCategory> ListAll()
        {
            return List();
        }
    }
}