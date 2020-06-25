using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class RequestReceivedApiLogRepository : EfCrudRepository<RequestReceivedApiLog>, IRequestReceivedApiLogRepository
    {
        public RequestReceivedApiLogRepository(ScaffoldContext context)
            : base(context)
        {
        }
    }
}