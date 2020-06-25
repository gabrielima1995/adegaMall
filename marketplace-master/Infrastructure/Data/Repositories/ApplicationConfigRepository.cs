using Domain;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class ApplicationConfigRepository : EfCrudRepository<ApplicationConfig>, IApplicationConfigRepository
    {
        public ApplicationConfigRepository(ScaffoldContext context) : base(context)
        {
        }

        public ApplicationConfig Get()
        {
            return CacheHelper.Get(Context.ApplicationConfig);
        }
    }
}
