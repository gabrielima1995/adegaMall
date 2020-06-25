using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class EcommerceHomeCategoryRepository : EfCrudRepository<EcommerceHomeCategory>, IEcommerceHomeCategoryRepository
    {
        public EcommerceHomeCategoryRepository(ScaffoldContext context) : base(context)
        {
        }
    }
}
