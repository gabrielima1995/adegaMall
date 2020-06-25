using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;

namespace Infrastructure.Data.Repositories
{
    public class WebhookCalledApiLogRepository : EfCrudRepository<WebhookCalledApiLog>, IWebhookCalledApiLogRepository
    {
        public WebhookCalledApiLogRepository(ScaffoldContext context)
            : base(context)
        {
        }
    }
}