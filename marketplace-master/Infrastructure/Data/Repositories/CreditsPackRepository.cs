using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class CreditsPackRepository : EfCrudRepository<CreditsPack>, ICreditsPackRepository
    {
        public CreditsPackRepository(ScaffoldContext context) : base(context)
        {
        }
    }
}
