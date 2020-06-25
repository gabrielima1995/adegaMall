using Domain.Entities.SubscriptionModule;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class PlanRepository : EfCrudRepository<Plan>, IPlanRepository
    {
        public PlanRepository(ScaffoldContext context) : base(context)
        {
        }
    }
}
