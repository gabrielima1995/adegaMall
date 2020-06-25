using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class StringRepository : EfCrudRepository<Str>, IStringRepository
    {
        public StringRepository(ScaffoldContext context) : base(context)
        {
        }
    }
}
